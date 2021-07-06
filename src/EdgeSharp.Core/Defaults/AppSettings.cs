using EdgeSharp.Core.Configuration;
using EdgeSharp.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EdgeSharp.Core.Defaults
{
    /// <summary>
    /// The default implementation of <see cref="IAppSettings"/>.
    /// </summary>
    public class AppSettings : IAppSettings
    {
        private DynamicDictionary _dynamicDictionary;

        /// <summary>
        /// Initializes a new instance of <see cref="AppSettings"/>.
        /// </summary>
        /// <param name="appName">The application name used to name the settings file.</param>
        public AppSettings(string appName = "edgesharp")
        {
            AppName = appName;
        }

        /// <inheritdoc />
        public string AppName { get; set; }

        /// <inheritdoc />
        public string SettingsFilePath { get; set; }

        /// <inheritdoc />
        public dynamic Settings
        {
            get
            {
                if (_dynamicDictionary == null)
                {
                    _dynamicDictionary = new DynamicDictionary();
                }

                return _dynamicDictionary;
            }
        }

        /// <inheritdoc />
        public virtual void Read(IConfiguration config)
        {
            try
            {
                var appSettingsFile = AppSettingInfo.GetSettingsFilePath(AppName);

                if (appSettingsFile == null)
                {
                    return;
                }

                var info = new FileInfo(appSettingsFile);
                if ((info.Exists) && info.Length > 0)
                {
                    using (StreamReader jsonReader = new StreamReader(appSettingsFile))
                    {
                        string json = jsonReader.ReadToEnd();
                        var options = new JsonSerializerOptions();
                        options.ReadCommentHandling = JsonCommentHandling.Skip;
                        options.AllowTrailingCommas = true;

                        var settingsJsonElement = JsonSerializer.Deserialize<JsonElement>(json, options);
                        var settingsDic = JsonToDictionary(settingsJsonElement);
                        _dynamicDictionary = new DynamicDictionary(settingsDic);
                    }
                }

                if (File.Exists(appSettingsFile))
                {
                    SettingsFilePath = appSettingsFile;
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        /// <inheritdoc />
        public virtual void Save(IConfiguration config)
        {
            try
            {
                if (_dynamicDictionary == null ||
                    _dynamicDictionary.Empty ||
                    _dynamicDictionary.Dictionary == null ||
                    _dynamicDictionary.Dictionary.Count == 0)
                {
                    return;
                }

                var appSettingsFile = SettingsFilePath;

                if (string.IsNullOrWhiteSpace(appSettingsFile))
                {
                    appSettingsFile = AppSettingInfo.GetSettingsFilePath(AppName, true);
                }

                if (appSettingsFile == null)
                {
                    return;
                }

                using (StreamWriter streamWriter = File.CreateText(appSettingsFile))
                {
                    try
                    {
                        var options = new JsonSerializerOptions();
                        options.ReadCommentHandling = JsonCommentHandling.Skip;
                        options.AllowTrailingCommas = true;

                        var jsonDic = JsonSerializer.Serialize(_dynamicDictionary.Dictionary, options);
                        streamWriter.Write(jsonDic);
                    }
                    catch (Exception exception)
                    {
                        Logger.Instance.Log.LogError(exception);
                        Logger.Instance.Log.LogWarning("If this is about cycle was detection please see - https://github.com/dotnet/corefx/issues/41288");
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Instance.Log.LogError(exception);
            }
        }

        private IDictionary<string, object> JsonToDictionary(JsonElement jsonElement)
        {
            var dic = new Dictionary<string, object>();

            foreach (var jsonProperty in jsonElement.EnumerateObject())
            {
                switch (jsonProperty.Value.ValueKind)
                {
                    case JsonValueKind.Null:
                        dic.Add(jsonProperty.Name, null);
                        break;
                    case JsonValueKind.Number:
                        dic.Add(jsonProperty.Name, jsonProperty.Value.GetDouble());
                        break;
                    case JsonValueKind.False:
                        dic.Add(jsonProperty.Name, false);
                        break;
                    case JsonValueKind.True:
                        dic.Add(jsonProperty.Name, true);
                        break;
                    case JsonValueKind.Undefined:
                        dic.Add(jsonProperty.Name, null);
                        break;
                    case JsonValueKind.String:
                        var strValue = jsonProperty.Value.GetString();
                        if (DateTime.TryParse(strValue, out DateTime date))
                        {
                            dic.Add(jsonProperty.Name, date);
                        }
                        else
                        {
                            dic.Add(jsonProperty.Name, strValue);
                        }

                        break;
                    case JsonValueKind.Object:
                        dic.Add(jsonProperty.Name, JsonToDictionary(jsonProperty.Value));
                        break;
                    case JsonValueKind.Array:
                        ArrayList objectList = new ArrayList();
                        foreach (JsonElement item in jsonProperty.Value.EnumerateArray())
                        {
                            switch (item.ValueKind)
                            {
                                case JsonValueKind.Null:
                                    objectList.Add(null);
                                    break;
                                case JsonValueKind.Number:
                                    objectList.Add(item.GetDouble());
                                    break;
                                case JsonValueKind.False:
                                    objectList.Add(false);
                                    break;
                                case JsonValueKind.True:
                                    objectList.Add(true);
                                    break;
                                case JsonValueKind.Undefined:
                                    objectList.Add(null);
                                    break;
                                case JsonValueKind.String:
                                    var itemValue = item.GetString();
                                    if (DateTime.TryParse(itemValue, out DateTime itemDate))
                                    {
                                        objectList.Add(itemDate);
                                    }
                                    else
                                    {
                                        objectList.Add(itemValue);
                                    }

                                    break;
                                default:
                                    objectList.Add(JsonToDictionary(item));
                                    break;
                            }
                        }
                        dic.Add(jsonProperty.Name, objectList);
                        break;
                }
            }

            return dic;
        }
    }
}