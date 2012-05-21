using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Xml;

namespace Services
{
    public sealed class LocalSelectableFileSettingsProvider : LocalFileSettingsProvider
    {
        private const string ApplicationSettingsGroupPrefix = "applicationSettings/";
        private static readonly object SyncRoot = new object();

        private static string _exeConfigFilename;

        public static string ExeConfigFilename
        {
            get { return _exeConfigFilename; }
            set
            {
                lock (SyncRoot)
                {
                    _exeConfigFilename = value;
                }
            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
        {
            var values = new SettingsPropertyValueCollection();

            var properties2 = (SettingsPropertyCollection)properties.Clone();

            foreach (SettingsProperty setting in properties)
            {
                if (setting.Attributes[typeof(SpecialSettingAttribute)] as SpecialSettingAttribute != null)
                    continue;

                var value = new SettingsPropertyValue(setting);

                var settings = ReadSettings(GetSectionName(context));

                if (settings.Contains(setting.Name))
                {
                    var ss = (StoredSetting)settings[setting.Name];
                    var valueString = ss.Value.InnerXml;

                    if (ss.SerializeAs == SettingsSerializeAs.String)
                        valueString = new XmlEscaper().Unescape(valueString);

                    value.SerializedValue = valueString;
                }
                else if (setting.DefaultValue != null)
                    value.SerializedValue = setting.DefaultValue;
                else
                    value.PropertyValue = null;

                value.IsDirty = false;

                values.Add(value);

                properties2.Remove(value.Name);
            }

            foreach (SettingsPropertyValue value in base.GetPropertyValues(context, properties2))
                values.Add(value);

            return values;
        }

        private static string GetSectionName(IDictionary context)
        {
            var groupName = (string)context["GroupName"];
            var key = (string)context["SettingsKey"];

            Debug.Assert(groupName != null, "SettingsContext did not have a GroupName!");

            var sectionName = groupName;

            if (!string.IsNullOrEmpty(key))
                sectionName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", sectionName, key);

            return XmlConvert.EncodeLocalName(sectionName);
        }

        private static IDictionary ReadSettings(string sectionName)
        {
            IDictionary settings = new Hashtable();

            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = ExeConfigFilename };
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var section = config.GetSection(ApplicationSettingsGroupPrefix + sectionName) as ClientSettingsSection;

            if (section != null)
                foreach (SettingElement setting in section.Settings)
                    settings[setting.Name] = new StoredSetting(setting.SerializeAs, setting.Value.ValueXml);

            return settings;
        }

        private class XmlEscaper
        {
            private readonly XmlDocument _doc;
            private readonly XmlElement _temp;

            public XmlEscaper()
            {
                _doc = new XmlDocument();
                _temp = _doc.CreateElement("temp");
            }

            public string Escape(string xmlString)
            {
                if (String.IsNullOrEmpty(xmlString))
                    return xmlString;
             
                _temp.InnerText = xmlString;
                return _temp.InnerXml;
            }

            public string Unescape(string escapedString)
            {
                if (String.IsNullOrEmpty(escapedString))
                    return escapedString;
             
                _temp.InnerXml = escapedString;
                return _temp.InnerText;
            }
        }

        private struct StoredSetting
        {
            public StoredSetting(SettingsSerializeAs serializeAs, XmlNode value)
            {
                SerializeAs = serializeAs;
                Value = value;
            }

            public readonly SettingsSerializeAs SerializeAs;
            public readonly XmlNode Value;
        }
    }
}
