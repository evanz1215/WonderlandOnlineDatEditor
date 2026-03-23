using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using WonderlandOnlineDatEditor.Core;
using WonderlandOnlineDatEditor.Parsers;

namespace WonderlandOnlineDatEditor
{
    public partial class MainWindow : Window
    {
        private DatFile? _datFile;
        private IList? _allRecords;
        private ICollectionView? _view;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Open Dat File",
                Filter = "Dat Files (*.dat)|*.Dat;*.dat|All Files (*.*)|*.*"
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                _datFile = DatFile.Load(dlg.FileName);
                LoadRecords();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRecords()
        {
            if (_datFile == null) return;

            _allRecords = _datFile.FileType switch
            {
                DatFileType.Item => _datFile.DecodeAll<ItemRecord>(),
                DatFileType.Npc => _datFile.DecodeAll<NpcRecord>(),
                DatFileType.Skill => _datFile.DecodeAll<SkillRecord>(),
                DatFileType.Scene => _datFile.DecodeAll<SceneRecord>(),
                DatFileType.Compound2 => _datFile.DecodeAll<Compound2Record>(),
                _ => null
            };

            if (_allRecords == null) return;

            dataGrid.ItemsSource = _allRecords;
            _view = CollectionViewSource.GetDefaultView(_allRecords);

            lblFileType.Text = _datFile.FileType.ToString();
            lblRecordCount.Text = $"{_datFile.RecordCount:N0} records";
            lblStatus.Text = _datFile.FilePath;
            lblFileSize.Text = $"{_datFile.RawData.Length:N0} bytes";
            btnSave.IsEnabled = true;
            btnExportCsv.IsEnabled = true;
            txtSearch.Text = "";
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_view == null || _allRecords == null) return;

            string search = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(search))
            {
                _view.Filter = null;
                lblRecordCount.Text = $"{_allRecords.Count:N0} records";
                return;
            }

            var props = _allRecords[0]!.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            _view.Filter = obj =>
            {
                foreach (var prop in props)
                {
                    var val = prop.GetValue(obj);
                    if (val == null) continue;
                    if (val is Array arr)
                    {
                        foreach (var item in arr)
                            if (item?.ToString()?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
                                return true;
                    }
                    else if (val.ToString()?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
                        return true;
                }
                return false;
            };

            int count = 0;
            foreach (var _ in _view) count++;
            lblRecordCount.Text = $"{count:N0} / {_allRecords.Count:N0} records";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_datFile == null || _allRecords == null) return;

            var dlg = new SaveFileDialog
            {
                Title = "Save Dat File",
                Filter = "Dat Files (*.dat)|*.Dat;*.dat",
                FileName = Path.GetFileName(_datFile.FilePath)
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                // For now, re-encode Item records; others save raw
                byte[] output;
                if (_datFile.FileType == DatFileType.Item)
                {
                    var items = _allRecords.Cast<ItemRecord>().ToList();
                    output = new byte[items.Count * 451];
                    for (int i = 0; i < items.Count; i++)
                        Array.Copy(items[i].Encode(), 0, output, i * 451, 451);
                }
                else
                {
                    output = _datFile.RawData;
                }

                // Backup existing file
                if (File.Exists(dlg.FileName))
                    File.Copy(dlg.FileName, dlg.FileName + ".bak", true);

                File.WriteAllBytes(dlg.FileName, output);
                MessageBox.Show($"Saved to {dlg.FileName}", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Save failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportCsv_Click(object sender, RoutedEventArgs e)
        {
            if (_allRecords == null || _allRecords.Count == 0) return;

            var dlg = new SaveFileDialog
            {
                Title = "Export CSV",
                Filter = "CSV Files (*.csv)|*.csv",
                FileName = Path.GetFileNameWithoutExtension(_datFile?.FilePath ?? "data") + ".csv"
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                var props = _allRecords[0]!.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var sb = new StringBuilder();

                // Header
                sb.AppendLine(string.Join(",", props.Select(p => p.Name)));

                // Rows
                foreach (var record in _allRecords)
                {
                    var values = props.Select(p =>
                    {
                        var val = p.GetValue(record);
                        if (val is Array arr)
                            return string.Join(";", arr.Cast<object>());
                        var s = val?.ToString() ?? "";
                        return s.Contains(',') || s.Contains('"') || s.Contains('\n')
                            ? $"\"{s.Replace("\"", "\"\"")}\""
                            : s;
                    });
                    sb.AppendLine(string.Join(",", values));
                }

                File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show($"Exported {_allRecords.Count} records to {dlg.FileName}", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
