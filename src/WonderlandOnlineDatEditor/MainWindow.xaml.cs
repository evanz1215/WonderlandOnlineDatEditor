using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        private readonly Dictionary<string, TabItem> _openTabs = new(StringComparer.OrdinalIgnoreCase);

        public MainWindow()
        {
            InitializeComponent();
        }

        private FileTab? CurrentTab =>
            tabControl.SelectedItem is TabItem ti && ti.Tag is FileTab ft ? ft : null;

        // ───────── Open ─────────

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Open Data File",
                Filter = "Dat Files (*.dat)|*.Dat;*.dat|MMG Files (*.mmg)|*.MMG;*.mmg|EMG Files (*.emg)|*.emg;*.Emg|MBTM Files (*.mbtm)|*.MBTM;*.mbtm|Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };
            if (dlg.ShowDialog() != true) return;

            OpenFile(dlg.FileName);
        }

        private async void OpenFile(string path)
        {
            // If already open, just switch to that tab
            if (_openTabs.TryGetValue(path, out var existing))
            {
                tabControl.SelectedItem = existing;
                return;
            }

            var tab = new FileTab(path);
            var fileInfo = new FileInfo(path);
            tab.FileSize = fileInfo.Length;
            tab.IsLoading = true;

            // Create the TabItem with a loading indicator first
            var tabItem = CreateTabItem(tab);
            _openTabs[path] = tabItem;
            tabControl.Items.Add(tabItem);
            tabControl.SelectedItem = tabItem;
            UpdateTabVisibility();

            try
            {
                // Load data on background thread
                await Task.Run(() => LoadFileData(tab, path));

                // Back on UI thread — bind the DataGrid
                if (tab.AllRecords != null && tab.AllRecords.Count > 0)
                {
                    var grid = GetDataGrid(tabItem);
                    if (grid != null)
                    {
                        grid.ItemsSource = tab.AllRecords;
                        tab.View = CollectionViewSource.GetDefaultView(tab.AllRecords);
                        tab.RecordCountText = $"{tab.AllRecords.Count:N0} records";
                    }
                }
                else
                {
                    tab.RecordCountText = "0 records";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Remove the failed tab
                CloseTabItem(tabItem);
                return;
            }
            finally
            {
                tab.IsLoading = false;
                // Hide loading overlay
                var loadingOverlay = FindLoadingOverlay(tabItem);
                if (loadingOverlay != null) loadingOverlay.Visibility = Visibility.Collapsed;
            }

            UpdateStatusBar(tab);
            UpdateToolbar(tab);
        }

        private void LoadFileData(FileTab tab, string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            string name = Path.GetFileName(path).ToLowerInvariant();

            if (ext == ".dat")
            {
                // Special dat files (non-XOR)
                if (name.Contains("talk"))
                {
                    var talk = TalkDatFile.Load(path);
                    tab.AllRecords = talk.ToRows();
                    tab.FileTypeLabel = $"Talk.dat ({talk.RecordCount:N0} records)";
                }
                else if (name.Contains("mark"))
                {
                    var mark = MarkDatFile.Load(path);
                    tab.AllRecords = mark.ToRows();
                    tab.FileTypeLabel = $"Mark.dat ({mark.RecordCount:N0} records)";
                }
                else if (name.Contains("formula"))
                {
                    var formula = FormulaDatFile.Load(path);
                    tab.AllRecords = formula.ToRows();
                    tab.FileTypeLabel = $"Formula.dat ({formula.Count} coefficients)";
                }
                else if (name.Contains("gec") || name.Contains("lbd"))
                {
                    var small = SmallDatFile.Load(path);
                    tab.AllRecords = small.ToRows();
                    tab.FileTypeLabel = $"{small.FileType} ({small.Count} records × {small.RecordSize}B)";
                }
                else
                {
                    // Try known XOR types
                    var detectedType = DatFileTypes.DetectType(name, new FileInfo(path).Length);
                    if (detectedType != null)
                    {
                        tab.DatFile = DatFile.Load(path);
                        tab.IsSaveable = true;
                        tab.AllRecords = tab.DatFile.FileType switch
                        {
                            DatFileType.Item => tab.DatFile.DecodeAll<ItemRecord>(),
                            DatFileType.Npc => tab.DatFile.DecodeAll<NpcRecord>(),
                            DatFileType.Skill => tab.DatFile.DecodeAll<SkillRecord>(),
                            DatFileType.Scene => tab.DatFile.DecodeAll<SceneRecord>(),
                            DatFileType.Compound2 => tab.DatFile.DecodeAll<Compound2Record>(),
                            _ => null
                        };
                        tab.FileTypeLabel = tab.DatFile.FileType.ToString();
                    }
                    else
                    {
                        // Unknown dat — raw hex
                        var raw = RawBinaryFile.Load(path);
                        tab.AllRecords = raw.ToRows();
                        tab.FileTypeLabel = raw.FileType + " (Raw)";
                    }
                }
            }
            else if (name.Contains("ground") && ext == ".mmg")
            {
                var ground = GroundMMGFile.Load(path);
                tab.AllRecords = ground.ToRows();
                tab.FileTypeLabel = "Ground.MMG";
            }
            else if (name.Contains("wem") && ext == ".mmg")
            {
                var wem = WemMMGFile.Load(path);
                tab.AllRecords = wem.ToRows();
                tab.FileTypeLabel = "Wem.MMG";
            }
            else if (ext == ".mmg")
            {
                var ground = GroundMMGFile.Load(path);
                tab.AllRecords = ground.ToRows();
                tab.FileTypeLabel = "MMG";
            }
            else if (ext == ".txt" && name.Contains("traffic"))
            {
                var traffic = TrafficSettingFile.Load(path);
                tab.AllRecords = traffic.ToRows();
                tab.FileTypeLabel = "TrafficSetting";
            }
            else if (ext == ".emg" || name.Contains("eve"))
            {
                var eve = EveEmgFile.Load(path);
                tab.AllRecords = eve.ToRows();
                tab.FileTypeLabel = $"Eve.emg ({eve.Npcs.Count} NPCs, {eve.Warps.Count} Warps)";
            }
            else
            {
                // Unknown format — raw hex viewer
                var raw = RawBinaryFile.Load(path);
                tab.AllRecords = raw.ToRows();
                tab.FileTypeLabel = raw.FileType + " (Raw)";
            }
        }

        // ───────── Tab UI helpers ─────────

        private TabItem CreateTabItem(FileTab tab)
        {
            // Tab header with close button
            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal };
            var headerText = new TextBlock
            {
                Text = tab.FileName,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 6, 0)
            };
            var closeBtn = new Button
            {
                Content = "\u2715",
                FontSize = 10,
                Padding = new Thickness(4, 1, 4, 1),
                VerticalAlignment = VerticalAlignment.Center,
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Foreground = (System.Windows.Media.Brush)FindResource("FgMuted"),
                Cursor = System.Windows.Input.Cursors.Hand,
                ToolTip = "Close tab"
            };
            closeBtn.Click += (_, _) =>
            {
                if (tabControl.Items.Cast<TabItem>().FirstOrDefault(t => t.Tag == tab) is TabItem ti)
                    CloseTabItem(ti);
            };
            headerPanel.Children.Add(headerText);
            headerPanel.Children.Add(closeBtn);

            // Content: Grid with DataGrid + loading overlay
            var contentGrid = new Grid();

            var dataGrid = new DataGrid
            {
                AutoGenerateColumns = true,
                IsReadOnly = !tab.IsSaveable,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                EnableRowVirtualization = true,
                EnableColumnVirtualization = true,
                MaxColumnWidth = 400,
            };
            VirtualizingPanel.SetIsVirtualizing(dataGrid, true);
            VirtualizingPanel.SetVirtualizationMode(dataGrid, VirtualizationMode.Recycling);
            contentGrid.Children.Add(dataGrid);

            // Loading overlay
            var loadingBorder = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromArgb(180, 30, 30, 46)),
                Name = "LoadingOverlay",
                Tag = "LoadingOverlay"
            };
            var loadingText = new TextBlock
            {
                Text = "Loading...",
                Foreground = (System.Windows.Media.Brush)FindResource("Accent"),
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            loadingBorder.Child = loadingText;
            contentGrid.Children.Add(loadingBorder);

            var tabItem = new TabItem
            {
                Header = headerPanel,
                Content = contentGrid,
                Tag = tab
            };
            return tabItem;
        }

        private static DataGrid? GetDataGrid(TabItem tabItem)
        {
            if (tabItem.Content is Grid g)
                return g.Children.OfType<DataGrid>().FirstOrDefault();
            return null;
        }

        private static Border? FindLoadingOverlay(TabItem tabItem)
        {
            if (tabItem.Content is Grid g)
                return g.Children.OfType<Border>().FirstOrDefault(b => b.Tag as string == "LoadingOverlay");
            return null;
        }

        private void CloseTabItem(TabItem tabItem)
        {
            if (tabItem.Tag is FileTab ft)
            {
                _openTabs.Remove(ft.FilePath);
                // Clear DataGrid binding to free memory
                var grid = GetDataGrid(tabItem);
                if (grid != null) grid.ItemsSource = null;
                ft.AllRecords = null;
                ft.View = null;
            }
            tabControl.Items.Remove(tabItem);
            UpdateTabVisibility();

            if (tabControl.Items.Count == 0)
            {
                lblFileType.Text = "";
                lblRecordCount.Text = "";
                lblStatus.Text = "No file loaded";
                lblFileSize.Text = "";
                btnSave.IsEnabled = false;
                btnExportCsv.IsEnabled = false;
                btnCloseTab.IsEnabled = false;
            }
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedItem is TabItem ti)
                CloseTabItem(ti);
        }

        private void UpdateTabVisibility()
        {
            bool hasTabs = tabControl.Items.Count > 0;
            tabControl.Visibility = hasTabs ? Visibility.Visible : Visibility.Collapsed;
            lblPlaceholder.Visibility = hasTabs ? Visibility.Collapsed : Visibility.Visible;
        }

        // ───────── Tab selection changed ─────────

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = CurrentTab;
            if (tab == null) return;

            UpdateStatusBar(tab);
            UpdateToolbar(tab);
            txtSearch.Text = tab.SearchText;
        }

        private void UpdateStatusBar(FileTab tab)
        {
            lblFileType.Text = tab.FileTypeLabel;
            lblRecordCount.Text = tab.RecordCountText;
            lblStatus.Text = tab.FilePath;
            lblFileSize.Text = $"{tab.FileSize:N0} bytes";
        }

        private void UpdateToolbar(FileTab tab)
        {
            btnSave.IsEnabled = tab.IsSaveable;
            btnExportCsv.IsEnabled = tab.AllRecords != null && tab.AllRecords.Count > 0;
            btnCloseTab.IsEnabled = true;
        }

        // ───────── Search ─────────

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tab = CurrentTab;
            if (tab?.View == null || tab.AllRecords == null) return;

            string search = txtSearch.Text.Trim();
            tab.SearchText = search;

            if (string.IsNullOrEmpty(search))
            {
                tab.View.Filter = null;
                tab.RecordCountText = $"{tab.AllRecords.Count:N0} records";
                lblRecordCount.Text = tab.RecordCountText;
                return;
            }

            var props = tab.AllRecords[0]!.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            tab.View.Filter = obj =>
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
            foreach (var _ in tab.View) count++;
            tab.RecordCountText = $"{count:N0} / {tab.AllRecords.Count:N0} records";
            lblRecordCount.Text = tab.RecordCountText;
        }

        // ───────── Save ─────────

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var tab = CurrentTab;
            if (tab?.DatFile == null || tab.AllRecords == null) return;

            var dlg = new SaveFileDialog
            {
                Title = "Save Dat File",
                Filter = "Dat Files (*.dat)|*.Dat;*.dat",
                FileName = Path.GetFileName(tab.DatFile.FilePath)
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                byte[] output;
                if (tab.DatFile.FileType == DatFileType.Item)
                {
                    var items = tab.AllRecords.Cast<ItemRecord>().ToList();
                    output = new byte[items.Count * 451];
                    for (int i = 0; i < items.Count; i++)
                        Array.Copy(items[i].Encode(), 0, output, i * 451, 451);
                }
                else
                {
                    output = tab.DatFile.RawData;
                }

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

        // ───────── Export CSV ─────────

        private void ExportCsv_Click(object sender, RoutedEventArgs e)
        {
            var tab = CurrentTab;
            if (tab?.AllRecords == null || tab.AllRecords.Count == 0) return;

            var dlg = new SaveFileDialog
            {
                Title = "Export CSV",
                Filter = "CSV Files (*.csv)|*.csv",
                FileName = Path.GetFileNameWithoutExtension(tab.FileName) + ".csv"
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                var props = tab.AllRecords[0]!.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var sb = new StringBuilder();

                sb.AppendLine(string.Join(",", props.Select(p => p.Name)));

                foreach (var record in tab.AllRecords)
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
                MessageBox.Show($"Exported {tab.AllRecords.Count} records to {dlg.FileName}", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
