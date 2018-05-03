using System.ComponentModel;
using System.Runtime.CompilerServices;
using CopyToLocales.Annotations;

namespace CopyToLocales
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Resources;
    using System.Windows;
    using System.Collections;
    using System.Windows.Forms;

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private const string EN = ".en.resx";

        private readonly Dictionary<string, ResXResourceReader> _sourceResourceReaders;
        private readonly Dictionary<string, ResXResourceReader> _targetResourceReaders;
        private readonly Dictionary<string, ResXResourceWriter> _targetResourceWriters;
        private readonly Dictionary<string, string> _sourceNames;

        private string _sourcePath;
        private string _targetPath;
        private string _sourcePathName;
        private string _targetPathName;

        public string SourcePathName
        {
            get => _sourcePathName;
            set
            {
                _sourcePathName = value;
                OnPropertyChanged(nameof(SourcePathName));
            }
        }

        public string TargetPathName
        {
            get => _targetPathName;
            set
            {
                _targetPathName = value;
                OnPropertyChanged(nameof(TargetPathName));
            }
        }

        public string SourcePath
        {
            get => _sourcePath;
            set
            {
                _sourcePath = value;
                OnPropertyChanged(nameof(SourcePath));
            }
        }

        public string TargetPath
        {
            get => _targetPath;
            set
            {
                _targetPath = value;
                OnPropertyChanged(nameof(TargetPath));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _sourceResourceReaders = new Dictionary<string, ResXResourceReader>();
            _targetResourceReaders = new Dictionary<string, ResXResourceReader>();
            _targetResourceWriters = new Dictionary<string, ResXResourceWriter>();
            _sourceNames = new Dictionary<string, string>();

            SourcePath = ControlSettings.Instance.LoadSetting(nameof(SourcePath));
            TargetPath = ControlSettings.Instance.LoadSetting(nameof(TargetPath));

            SourcePathName = ControlSettings.Instance.LoadSetting(nameof(SourcePathName));
            TargetPathName = ControlSettings.Instance.LoadSetting(nameof(TargetPathName));

            Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            ControlSettings.Instance.SaveFolderForHistory(nameof(SourcePath), SourcePath);
            ControlSettings.Instance.SaveFolderForHistory(nameof(TargetPath), TargetPath);

            ControlSettings.Instance.SaveFolderForHistory(nameof(SourcePathName), SourcePathName);
            ControlSettings.Instance.SaveFolderForHistory(nameof(TargetPathName), TargetPathName);
        }

        /// <summary>
        /// Указать папку - источник ресурсов.
        /// </summary>
        private void SourceOpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderPath(true);
        }

        /// <summary>
        /// Указать папку - получатель ресурсов.
        /// </summary>
        private void TargetOpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderPath(false);
        }

        /// <summary>
        /// Указать папку.
        /// </summary>
        /// <param name="isSource">Является ли указываемся папка источником.</param>
        private void OpenFolderPath(bool isSource)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (!string.IsNullOrEmpty(SourcePath) && isSource)
                folderBrowserDialog.SelectedPath = SourcePath;
            else if (!isSource && !string.IsNullOrEmpty(TargetPath))
                folderBrowserDialog.SelectedPath = TargetPath;

            folderBrowserDialog.Description = isSource ? "Укажите папку, откуда брать локализацию" : "Укажите папку, куда записывать локализацию";
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (isSource)
                    SourcePath = folderBrowserDialog.SelectedPath;
                else
                    TargetPath = folderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Инициализировать ресурсные файлы.
        /// </summary>
        /// <param name="isSource">Является ли указываемся папка источником.</param>
        private void InitReaders(bool isSource)
        {
            var allResx = Directory.GetFiles(isSource ? SourcePath : TargetPath).Where(x =>
                (isSource && (!string.IsNullOrEmpty(SourcePathName) && x.Contains(SourcePathName) || string.IsNullOrEmpty(SourcePathName))
                 || !isSource && (!string.IsNullOrEmpty(TargetPathName) && x.Contains(TargetPathName) || string.IsNullOrEmpty(TargetPathName))) && x.EndsWith(".resx"));

            foreach (var resx in allResx)
            {
                var rsx = resx.Split('.');
                string name = string.Empty;
                if (rsx.Length == 2)
                    name = $"ru.{rsx[1]}"; //Используется для русского языка.
                else if (rsx.Length == 3)
                    name = $".{rsx[1]}.{rsx[2]}";

                if (isSource)
                {
                    _sourceResourceReaders.Add(name, new ResXResourceReader(resx));
                }
                else
                {
                    _targetResourceReaders.Add(name, new ResXResourceReader(resx));
                    _targetResourceWriters.Add(name, new ResXResourceWriter(resx));
                }
            }
        }

        /// <summary>
        /// Установить старый - новый ключ ресурса.
        /// </summary>
        private bool SetKeyNames()
        {
            var sourceNames = SourceItemName.Text.Replace(" ", string.Empty).Split(',');
            var targetNames = TargetItemName.Text.Replace(" ", string.Empty).Split(',');

            if (sourceNames.Length != targetNames.Length)
                return false;

            for (int i = 0; i < sourceNames.Length; i++)
            {
                for (int j = 0; j < targetNames.Length; j++)
                {
                    if (i == j)
                    {
                        LogLostBox.Items.Add($"Старый ключ: {sourceNames[i]}, новый ключ: {targetNames[j]}");
                        _sourceNames.Add(sourceNames[i], targetNames[j]);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Начать копирование значений.
        /// </summary>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            LogLostBox.Items.Clear();
           
            if (string.IsNullOrEmpty(SourcePath))
                OpenFolderPath(true);

            if (string.IsNullOrEmpty(TargetPath))
                OpenFolderPath(false);

            _sourceResourceReaders.Clear();
            _targetResourceReaders.Clear();
            _targetResourceWriters.Clear();

            InitReaders(true);
            InitReaders(false);

            _sourceNames.Clear();

            if (!SetKeyNames())
                return;

            Save();

            LogLostBox.Items.Add("Выполнено");
            System.Windows.Forms.MessageBox.Show("Выполнено");
        }

        private void Save()
        {
            foreach (var item in _sourceResourceReaders)
            {
                LogLostBox.Items.Add($"Выбран файл локализации: {item.Key}");
                SaveOnceFile(item.Key, item.Value, _targetResourceReaders[item.Key], _targetResourceWriters[item.Key]);
            }
        }

        /// <summary>
        /// Получить значение для английского языка.
        /// </summary>
        private object GetEnDefaultValue(string sourceName)
        {
            foreach (var item in _sourceResourceReaders)
            {
                if (item.Key.Equals(EN))
                {
                    foreach (DictionaryEntry value in item.Value)
                    {
                        if (value.Key.Equals(sourceName))
                        {
                            return value.Value;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Получить ключ по значению.
        /// </summary>
        private object GetKeyFromValue(object sourceValue, string resxLang)
        {
            foreach (var item in _targetResourceReaders)
            {
                if (item.Key.Equals(resxLang))
                {
                    foreach (DictionaryEntry value in item.Value)
                    {
                        if (value.Value.Equals(sourceValue))
                        {
                            return value.Key;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Сохранение в файл ресурсов.
        /// </summary>
        private void SaveOnceFile(string itemKey, ResXResourceReader sourceResourceReader, ResXResourceReader targetResourceReader, ResXResourceWriter targetResourceWriter)
        {
            var newValue = new Dictionary<string, string>();
            foreach (DictionaryEntry item in sourceResourceReader)
            {
                foreach (var sourceName in _sourceNames)
                {
                    if (!item.Key.Equals(sourceName.Key))
                        continue;

                    object enValue = GetEnDefaultValue(sourceName.Key);

                    object valueInResx = GetKeyFromValue(item.Value, itemKey);

                    if (valueInResx != null)
                    {
                        LogLostBox.Items.Add($"Значение {item.Value} для файла {itemKey} доступно по ключу {valueInResx}.");
                        continue;
                    }

                    // TODO Добавление в  Locale.Designer.cs для быстрого доступа к данным ресурсам.
                    LogLostBox.Items.Add($"Получено значение по ключу {item.Key} со значением: {item.Value} для файла {itemKey}");

                    // Переносить для английского языка в любом случае.
                    // Если значение в текущем языке совпадет со значеним для ангийского языка,
                    // значит данная фраза не локализована и нет необходимости ее переносить.
                    if (enValue == null || !item.Value.Equals(enValue) || itemKey == EN)
                        newValue.Add(sourceName.Value, item.Value.ToString());
                }
            }

            if (newValue.Count > 0)
                SaveNewValue(targetResourceReader, targetResourceWriter, newValue);
        }

        /// <summary>
        /// Записать зничения в файл.
        /// </summary>
        private void SaveNewValue(ResXResourceReader targetResourceReader, ResXResourceWriter targetResourceWriter, Dictionary<string, string> keyValuePairss)
        {
            try
            {
                var node = targetResourceReader.GetEnumerator();
                while (node.MoveNext())
                    targetResourceWriter.AddResource(node.Key.ToString(), node.Value.ToString());

                foreach (var keyValuePair in keyValuePairss)
                {
                    targetResourceWriter.AddResource(new ResXDataNode(keyValuePair.Key, keyValuePair.Value));
                    LogLostBox.Items.Add($"Добавлен ключ: {keyValuePair.Key} со значением: {keyValuePair.Value}");
                }

                targetResourceWriter.Generate();
                targetResourceWriter.Close();

                LogLostBox.Items.Add($"Все значения записаны.");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return;
            }
        }

        /// <summary>
        /// Закрыть программу.
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
