using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AC1SaveExplorer
{
    // ----- raw dump shape (flexible: properties/values are kept as JsonElement
    // since the dumper's schema varies per entry) -----

    public class SaveDump
    {
        [JsonPropertyName("fileSize")] public long FileSize { get; set; }
        [JsonPropertyName("magic")] public string? Magic { get; set; }
        [JsonPropertyName("objectCount")] public int ObjectCount { get; set; }
        [JsonPropertyName("version")] public int Version { get; set; }
        [JsonPropertyName("objects")] public List<SaveObjectRaw> Objects { get; set; } = new();
    }

    public class SaveObjectRaw
    {
        [JsonPropertyName("classID")] public long ClassID { get; set; }
        [JsonPropertyName("index")] public int Index { get; set; }
        [JsonPropertyName("propertyCount")] public int PropertyCount { get; set; }
        [JsonPropertyName("properties")] public List<JsonElement> Properties { get; set; } = new();
        [JsonPropertyName("values")] public List<JsonElement> Values { get; set; } = new();
    }

    // ----- hash dictionaries -----

    public class ClassNameEntry
    {
        public List<string> Names { get; set; } = new();
        public string Category { get; set; } = "uncategorized"; // "save" | "mission" | "uncategorized"
    }

    // ----- view-facing models -----

    public class PropertyRowVm
    {
        public string DisplayName { get; set; } = "";
        public bool IsNamed { get; set; }
        public string RawId { get; set; } = "";
        public string Type { get; set; } = "?";
        public string Value { get; set; } = "";
    }

    public class SaveObjectVm : INotifyPropertyChanged
    {
        public long ClassID { get; }
        public int Index { get; }
        public string HashHex { get; }
        public SaveObjectRaw Raw { get; }

        private string _name = "";
        public string Name { get => _name; set { _name = value; OnChanged(nameof(Name)); OnChanged(nameof(DisplayName)); OnChanged(nameof(IsUnnamed)); } }

        public bool IsUnnamed => string.IsNullOrWhiteSpace(Name);
        public string DisplayName => IsUnnamed ? $"unnamed object · classID {ClassID}" : Name;

        private string _category = "uncategorized";
        public string Category { get => _category; set { _category = value; OnChanged(nameof(Category)); } }

        public int FieldCount => Raw.Properties.Count;

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; OnChanged(nameof(IsExpanded)); if (value) BuildRows(); }
        }

        public ObservableCollection<PropertyRowVm> Rows { get; } = new();

        private readonly Func<SaveObjectRaw, ObservableCollection<PropertyRowVm>, string> _rowBuilder;
        private bool _built;

        public SaveObjectVm(SaveObjectRaw raw, Func<SaveObjectRaw, ObservableCollection<PropertyRowVm>, string> rowBuilder)
        {
            Raw = raw;
            ClassID = raw.ClassID;
            Index = raw.Index;
            HashHex = "0x" + ((uint)ClassID).ToString("X8");
            _rowBuilder = rowBuilder;
        }

        public void BuildRows(bool force = false)
        {
            if (_built && !force) return;
            _rowBuilder(Raw, Rows);
            _built = true;
        }

        public void InvalidateRows() { _built = false; if (IsExpanded) BuildRows(true); }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
