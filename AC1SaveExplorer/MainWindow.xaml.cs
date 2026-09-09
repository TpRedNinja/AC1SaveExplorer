using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace AC1SaveExplorer
{
    public partial class MainWindow : Window
    {
        // key = classID as string (for class names) / property id as string (for prop hashes)
        private Dictionary<string, List<string>> _propHashes = new();
        private Dictionary<string, ClassNameEntry> _classNames = new();

        private SaveDump? _dump;
        private List<SaveObjectVm> _allObjects = new();
        private readonly ObservableCollection<SaveObjectVm> _visible = new();
        private HashSet<string> _missionSignatures = new();

        private string _activeTab = "save";
        private string _search = "";

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            WriteIndented = true
        };

        private static string SettingsDir =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AC1SaveExplorer");
        private static string PropHashesPath => Path.Combine(SettingsDir, "prop_hashes.json");
        private static string ClassNamesPath => Path.Combine(SettingsDir, "class_names.json");

        public MainWindow()
        {
            InitializeComponent();
            ObjectList.ItemsSource = _visible;

            LoadSeedDictionaries();
            LoadPersistedDictionaries();

            // start with a tiny built-in sample so the window isn't empty on first run
            _dump = BuildSampleDump();
            DumpStatus.Text = "Showing a small built-in sample. Load a real dump.json to replace it.";
            RebuildObjects();
            RefreshView();
        }

        // ================= default / seed dictionaries =================

        private void LoadSeedDictionaries()
        {
            // property-hash dictionary shipped by AC1SaveTool (github.com/bloxtbc/AC1SaveTool, MIT)
            const string defaultPropHashesJson = """
{"59534803":["CityMarker"],"182352896":["SpawnMode"],"245999962":["MountIsChosen"],"356220115":["AltairRankSaved"],"418166524":["PersistedIDs"],"440541947":["ResetMonitor"],"494490051":["CitiesInKingdom"],"529056539":["Options"],"550671715":["ShowCtrlScheme"],"561800778":["DaggerShoulderCount"],"588182160":["MountIndex"],"782802969":["Monitor"],"822170364":["Accomplishments"],"864635854":["HorseMonitorData"],"955405642":["MainDaggerBeltCount"],"988155343":["SelectedWeapon"],"1027654762":["Vibration"],"1142451046":["Count"],"1170604816":["FreeMissionInformerCount"],"1184746767":["AssassinMatrix"],"1207356937":["MobileSpawnerPos"],"1275278747":["DaggerBeltCount"],"1361138523":["SetByPlayerMarker"],"1496786106":["CheckPointOnlySemaphore"],"1501289652":["AlwaysSaveWorldID"],"1534400570":["InvertXLook"],"1551827799":["AssassinSaved"],"1573852237":["FreeMissionScholarsCount"],"1575644396":["SFXVolume"],"1649606143":["State"],"1669243737":["XLookSensitivity"],"1670108668":["IsResolved"],"1712664970":["InvertYLook"],"1849811038":["NothingIsTrueEverythingIsPermitted"],"1922168126":["MarkerType"],"1958527870":["AssassinOnMount"],"2001406352":["MissionReplay"],"2005069724":["UsedEvents"],"2096795148":["MarkerPosition"],"2105823925":["MobileSpawnerDir"],"2113130176":["ShowBlood"],"2178661708":["TotalNumSynchBlocks"],"2181900433":["HoldToFocus"],"2379497903":["Configs"],"2385831596":["Markers"],"2396801049":["MainSaveWorldID"],"2407447650":["MapWorldInfo"],"2465949767":["NoSaveSemaphore"],"2489875330":["FreeMissionVigilantesCount"],"2499365691":["GlobalTolerance"],"2519455027":["Brightness"],"2702571053":["Components"],"2737467230":["UnlockedBlocksCount"],"2747163608":["VoiceVolume"],"2785163220":["DaggerLegCount"],"2797770677":["CancelMonitor"],"2867239103":["ToleranceSaved"],"2871065487":["OtherMonitors"],"2879431345":["NumSynchBlocks"],"2911639119":["MainDaggerShoulderCount"],"2916176588":["OutMonitor"],"2969371692":["FogCells"],"3068249190":["OptionalObjectivesCount"],"3125686489":["IsAllUnfogged"],"3144687493":["ShowGPS"],"3155981222":["CurrentSaveWorldID"],"3161216672":["IsKnown"],"3197466611":["AltairMarker"],"3246730187":["MountIsInWorld"],"3440784072":["YLookSensitivity"],"3494805540":["AltairCurrentRank"],"3575369835":["MusicVolume"],"3584634421":["ShowWeaponIcon"],"3622219230":["CurrentMemoryBlock"],"3664481400":["DefaultCompassMarker"],"3678460294":["AltairNextRank"],"3734200949":["xDE936275"],"3776449165":["TextId"],"3801967644":["InMonitor"],"3847088039":["HighTolerance"],"3870325847":["MainDaggerLegCount"],"3872379813":["AutomaticMarkers"],"3990271243":["ShowSyncBar"],"4272727674":["OtherMonitor"],"882350721":["MissionStatus"],"1379586241":["IsCompleted"],"1842287097":["Unknown (mission)"]}
""";
            try
            {
                _propHashes = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(defaultPropHashesJson) ?? new();
            }
            catch { _propHashes = new(); }

            // seed class/object names: the 5 confirmed mission hashes + the PlayerSaveData object
            _classNames = new Dictionary<string, ClassNameEntry>
            {
                ["1005212782"] = new ClassNameEntry { Names = new() { "M02_SolomonTemple.MissionItemList" }, Category = "mission" },
                ["1040056258"] = new ClassNameEntry { Names = new() { "M05_HorseTutorial.MissionItemList" }, Category = "mission" },
                ["1045371048"] = new ClassNameEntry { Names = new() { "M06_MasyafUnderAttack.MissionItemList" }, Category = "mission" },
                ["1165355488"] = new ClassNameEntry { Names = new() { "Close_Tower_Trap.MissionItemSceneSequencer" }, Category = "mission" },
                ["2927099110"] = new ClassNameEntry { Names = new() { "Unload MB1.MissionItemChangeWorld" }, Category = "mission" },
                ["3003"] = new ClassNameEntry { Names = new() { "PlayerSaveData (global save state - holds CurrentSaveWorldID etc.)" }, Category = "save" },
            };
        }

        private void LoadPersistedDictionaries()
        {
            try
            {
                if (File.Exists(PropHashesPath))
                {
                    var loaded = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(File.ReadAllText(PropHashesPath));
                    if (loaded != null)
                        foreach (var kv in loaded) _propHashes[kv.Key] = kv.Value;
                }
                if (File.Exists(ClassNamesPath))
                {
                    var loaded = JsonSerializer.Deserialize<Dictionary<string, ClassNameEntry>>(File.ReadAllText(ClassNamesPath));
                    if (loaded != null)
                        foreach (var kv in loaded) _classNames[kv.Key] = kv.Value;
                }
                HashStatus.Text = $"{_propHashes.Count} property names, {_classNames.Count} object names loaded.";
            }
            catch (Exception ex)
            {
                HashStatus.Text = "Could not load saved dictionaries: " + ex.Message;
            }
        }

        private void PersistDictionaries()
        {
            try
            {
                Directory.CreateDirectory(SettingsDir);
                File.WriteAllText(PropHashesPath, JsonSerializer.Serialize(_propHashes, JsonOpts));
                File.WriteAllText(ClassNamesPath, JsonSerializer.Serialize(_classNames, JsonOpts));
            }
            catch (Exception ex)
            {
                HashStatus.Text = "Could not autosave dictionaries: " + ex.Message;
            }
        }

        private SaveDump BuildSampleDump()
        {
            // tiny built-in example so the window shows something real on first launch
            var json = """
            {
              "fileSize": 177299, "magic": "0x1234FEDC", "objectCount": 3, "version": 15,
              "objects": [
                { "classID": 3003, "index": 2, "propertyCount": 2,
                  "properties": [3155981222, 3622219230],
                  "values": [417742833, 1] },
                { "classID": 1005212782, "index": 334, "propertyCount": 3,
                  "properties": [882350721, 1379586241, 1842287097],
                  "values": [3, true, false] },
                { "classID": 1040056258, "index": 338, "propertyCount": 3,
                  "properties": [882350721, 1379586241, 1842287097],
                  "values": [0, false, false] }
              ]
            }
            """;
            return JsonSerializer.Deserialize<SaveDump>(json) ?? new SaveDump();
        }

        // ================= categorization / object building =================

        private string CategoryFor(long classID)
        {
            var key = classID.ToString();
            if (_classNames.TryGetValue(key, out var entry) && !string.IsNullOrWhiteSpace(entry.Category))
                return entry.Category;
            return classID > 0 && classID < 100000 ? "save" : "uncategorized";
        }

        private string NameFor(long classID)
        {
            var key = classID.ToString();
            if (_classNames.TryGetValue(key, out var entry) && entry.Names.Count > 0)
                return entry.Names[0];
            return "";
        }

        private void RebuildObjects()
        {
            _allObjects = new List<SaveObjectVm>();
            if (_dump == null) return;

            foreach (var raw in _dump.Objects)
            {
                var vm = new SaveObjectVm(raw, BuildRowsFor)
                {
                    Category = CategoryFor(raw.ClassID),
                    Name = NameFor(raw.ClassID)
                };
                _allObjects.Add(vm);
            }
            ComputeMissionSignatures();
            UpdateCounts();
        }

        // a "signature" is the sorted set of property ids an object carries — every AC1 mission
        // object we've confirmed so far (MissionStatus/IsCompleted/Unknown) shares the exact same
        // three property ids, so any other object with that same signature is very likely also
        // a mission entry we just haven't named yet.
        private static string? ExtractPropId(JsonElement p)
        {
            if (p.ValueKind == JsonValueKind.Number) return p.GetRawText();
            if (p.ValueKind == JsonValueKind.Object)
            {
                var idEl = TryGetAny(p, "id", "hash", "propertyId", "nameHash");
                if (idEl != null) return idEl.Value.GetRawText();
                return p.GetRawText();
            }
            if (p.ValueKind == JsonValueKind.String) return p.GetString();
            return null;
        }

        private static string SignatureOf(SaveObjectRaw raw)
        {
            var ids = raw.Properties
                .Select(ExtractPropId)
                .Where(id => id != null)
                .OrderBy(id => id, StringComparer.Ordinal);
            return string.Join("|", ids);
        }

        private void ComputeMissionSignatures()
        {
            _missionSignatures = _allObjects
                .Where(o => o.Category == "mission")
                .Select(o => SignatureOf(o.Raw))
                .Where(sig => !string.IsNullOrEmpty(sig))
                .ToHashSet();
        }

        private bool IsCandidate(SaveObjectVm o) =>
            o.Category != "mission" && _missionSignatures.Contains(SignatureOf(o.Raw));

        private void UpdateCounts()
        {
            CountSave.Text = _allObjects.Count(o => o.Category == "save").ToString();
            CountMission.Text = _allObjects.Count(o => o.Category == "mission").ToString();
            CountUncat.Text = _allObjects.Count(o => o.Category == "uncategorized").ToString();
            CountCandidates.Text = _allObjects.Count(IsCandidate).ToString();
        }

        // builds the property rows for one object lazily (called when its Expander opens)
        private string BuildRowsFor(SaveObjectRaw raw, ObservableCollection<PropertyRowVm> into)
        {
            into.Clear();
            int count = Math.Max(raw.Properties.Count, raw.Values.Count);
            for (int i = 0; i < count; i++)
            {
                string rawId = "?";
                string type = "?";
                bool isNamed = false;
                string displayName;

                if (i < raw.Properties.Count)
                {
                    var p = raw.Properties[i];
                    if (p.ValueKind == JsonValueKind.Number)
                    {
                        rawId = p.GetRawText();
                        if (_propHashes.TryGetValue(rawId, out var names) && names.Count > 0)
                        {
                            isNamed = true;
                            displayName = names[0];
                        }
                        else displayName = $"prop_{rawId}";
                    }
                    else if (p.ValueKind == JsonValueKind.Object)
                    {
                        string? idStr = TryGetAny(p, "id", "hash", "propertyId", "nameHash")?.GetRawText();
                        rawId = idStr ?? p.GetRawText();
                        string? typeStr = TryGetAny(p, "type", "valueType")?.ToString();
                        type = typeStr ?? type;
                        if (idStr != null && _propHashes.TryGetValue(idStr, out var names) && names.Count > 0)
                        {
                            isNamed = true;
                            displayName = names[0];
                        }
                        else displayName = $"prop_{rawId}";
                    }
                    else
                    {
                        rawId = p.GetRawText();
                        displayName = $"prop_{i}";
                    }
                }
                else
                {
                    displayName = $"prop_{i}";
                }

                string valueStr = i < raw.Values.Count ? DescribeValue(raw.Values[i], ref type) : "";

                into.Add(new PropertyRowVm
                {
                    DisplayName = displayName,
                    IsNamed = isNamed,
                    RawId = rawId,
                    Type = type,
                    Value = valueStr
                });
            }
            return "ok";
        }

        private static JsonElement? TryGetAny(JsonElement obj, params string[] names)
        {
            foreach (var n in names)
                if (obj.TryGetProperty(n, out var v)) return v;
            return null;
        }

        private static string DescribeValue(JsonElement v, ref string type)
        {
            switch (v.ValueKind)
            {
                case JsonValueKind.Number:
                    if (type == "?") type = v.TryGetInt64(out _) ? "int" : "float";
                    return v.GetRawText();
                case JsonValueKind.True:
                case JsonValueKind.False:
                    if (type == "?") type = "bool";
                    return v.GetBoolean().ToString();
                case JsonValueKind.String:
                    if (type == "?") type = "string";
                    return v.GetString() ?? "";
                case JsonValueKind.Array:
                    if (type == "?") type = "array";
                    return v.GetRawText();
                case JsonValueKind.Object:
                    if (type == "?") type = "object";
                    return v.GetRawText();
                case JsonValueKind.Null:
                    return "null";
                default:
                    return v.GetRawText();
            }
        }

        // ================= filtering / view =================

        private void RefreshView()
        {
            _visible.Clear();
            IEnumerable<SaveObjectVm> query = _activeTab == "candidates"
                ? _allObjects.Where(IsCandidate)
                : _allObjects.Where(o => o.Category == _activeTab);

            if (!string.IsNullOrWhiteSpace(_search))
            {
                var s = _search.Trim();
                query = query.Where(o =>
                    o.HashHex.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                    o.ClassID.ToString().Contains(s) ||
                    o.DisplayName.Contains(s, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var o in query) _visible.Add(o);
        }

        private void Tab_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag is string tag)
            {
                _activeTab = tag;
                RefreshView();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _search = SearchBox.Text;
            RefreshView();
        }

        // ================= dump loading =================

        private void BtnLoadDump_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Save dump JSON (*.json)|*.json|All files (*.*)|*.*" };
            if (dlg.ShowDialog() != true) return;

            try
            {
                var text = File.ReadAllText(dlg.FileName);
                var dump = JsonSerializer.Deserialize<SaveDump>(text);
                if (dump == null || dump.Objects.Count == 0)
                {
                    DumpStatus.Text = "That file didn't parse into any objects — check it's an AC1SaveTool-style dump.";
                    return;
                }
                _dump = dump;
                RebuildObjects();
                RefreshView();
                DumpStatus.Text = $"Loaded {Path.GetFileName(dlg.FileName)} — {dump.ObjectCount} objects, {dump.FileSize} bytes, version {dump.Version}.";
            }
            catch (Exception ex)
            {
                DumpStatus.Text = "Failed to load dump: " + ex.Message;
            }
        }

        // ================= hash dictionary loading / exporting =================

        private void BtnLoadProps_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Hash dictionary JSON (*.json)|*.json|All files (*.*)|*.*" };
            if (dlg.ShowDialog() != true) return;
            try
            {
                var loaded = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(File.ReadAllText(dlg.FileName));
                if (loaded == null) throw new Exception("empty or invalid file");
                foreach (var kv in loaded) _propHashes[kv.Key] = kv.Value;
                foreach (var obj in _allObjects) obj.InvalidateRows();
                PersistDictionaries();
                HashStatus.Text = $"Merged {loaded.Count} property names ({_propHashes.Count} total).";
            }
            catch (Exception ex)
            {
                HashStatus.Text = "Failed to load property hashes: " + ex.Message;
            }
        }

        private void BtnLoadClasses_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Object name JSON (*.json)|*.json|All files (*.*)|*.*" };
            if (dlg.ShowDialog() != true) return;
            try
            {
                var loaded = JsonSerializer.Deserialize<Dictionary<string, ClassNameEntry>>(File.ReadAllText(dlg.FileName));
                if (loaded == null) throw new Exception("empty or invalid file");
                foreach (var kv in loaded) _classNames[kv.Key] = kv.Value;
                RebuildObjects();
                RefreshView();
                PersistDictionaries();
                HashStatus.Text = $"Merged {loaded.Count} object names ({_classNames.Count} total).";
            }
            catch (Exception ex)
            {
                HashStatus.Text = "Failed to load object names: " + ex.Message;
            }
        }

        private void BtnExportProps_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "JSON (*.json)|*.json", FileName = "prop_hashes.json" };
            if (dlg.ShowDialog() != true) return;
            try
            {
                File.WriteAllText(dlg.FileName, JsonSerializer.Serialize(_propHashes, JsonOpts));
                HashStatus.Text = $"Exported {_propHashes.Count} property names to {Path.GetFileName(dlg.FileName)}.";
            }
            catch (Exception ex)
            {
                HashStatus.Text = "Export failed: " + ex.Message;
            }
        }

        private void BtnExportClasses_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Filter = "JSON (*.json)|*.json", FileName = "class_names.json" };
            if (dlg.ShowDialog() != true) return;
            try
            {
                File.WriteAllText(dlg.FileName, JsonSerializer.Serialize(_classNames, JsonOpts));
                HashStatus.Text = $"Exported {_classNames.Count} object names to {Path.GetFileName(dlg.FileName)}.";
            }
            catch (Exception ex)
            {
                HashStatus.Text = "Export failed: " + ex.Message;
            }
        }

        // ================= per-object naming/categorization =================

        private void CopyHash_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true; // don't let this bubble up and toggle the Expander
            if (sender is not Button btn || btn.Tag is not SaveObjectVm vm) return;
            try
            {
                Clipboard.SetText(vm.HashHex);
                HashStatus.Text = $"Copied {vm.HashHex} to clipboard.";
            }
            catch (Exception ex)
            {
                HashStatus.Text = "Copy failed: " + ex.Message;
            }
        }

        private void SaveObjectName_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not SaveObjectVm vm) return;

            var key = vm.ClassID.ToString();
            if (!_classNames.TryGetValue(key, out var entry))
            {
                entry = new ClassNameEntry();
                _classNames[key] = entry;
            }
            entry.Category = vm.Category;
            if (!string.IsNullOrWhiteSpace(vm.Name))
            {
                entry.Names.Remove(vm.Name);
                entry.Names.Insert(0, vm.Name);
            }

            // re-apply this classID's category/name to every object sharing it (mission lists etc. repeat)
            foreach (var obj in _allObjects.Where(o => o.ClassID == vm.ClassID))
            {
                obj.Category = vm.Category;
                if (!string.IsNullOrWhiteSpace(vm.Name)) obj.Name = vm.Name;
            }

            ComputeMissionSignatures();
            UpdateCounts();
            RefreshView();
            PersistDictionaries();
            HashStatus.Text = $"Saved classID {vm.ClassID} as \"{vm.Name}\" ({vm.Category}).";
        }
    }
}
