using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using WonderlandOnlineDatEditor.Core;

namespace WonderlandOnlineDatEditor;

/// <summary>
/// Holds all state for one opened file tab: records, view, search, metadata.
/// </summary>
public class FileTab : INotifyPropertyChanged
{
    public string FilePath { get; }
    public string FileName { get; }
    public string FileTypeLabel { get; set; } = "";
    public long FileSize { get; set; }
    public DatFile? DatFile { get; set; }
    public IList? AllRecords { get; set; }
    public ICollectionView? View { get; set; }
    public bool IsSaveable { get; set; }

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value) return;
            _searchText = value;
            OnPropertyChanged(nameof(SearchText));
        }
    }

    private string _recordCountText = "";
    public string RecordCountText
    {
        get => _recordCountText;
        set
        {
            if (_recordCountText == value) return;
            _recordCountText = value;
            OnPropertyChanged(nameof(RecordCountText));
        }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading == value) return;
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    public FileTab(string filePath)
    {
        FilePath = filePath;
        FileName = System.IO.Path.GetFileName(filePath);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
