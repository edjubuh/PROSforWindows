using Newtonsoft.Json;
using MahApps.Metro;
using System;
using System.Windows;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace PROSforWindows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ThemeManager.AddAccent("PurdueAccent", new Uri("pack://application:,,,/Resources/PurdueAccent.xaml"));
            using (StreamReader reader = new StreamReader(App.GetContentStream(new Uri("/settings.json", UriKind.Relative)).Stream))
            {
                var _JObject = JObject.Parse(reader.ReadToEnd());

                foreach (JProperty property in _JObject.Children())
                {
                    dynamic _prop = property.Value;

                    try
                    {
                        Application.Current.Properties.Add(property.Name,
                            JsonConvert.DeserializeObject(_prop.value.ToString(), Type.GetType((string)(_prop.type))));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("There was an error reading the settings file.", ex);
                    }
                }
            }
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            JObject obj = new JObject();
            foreach (DictionaryEntry property in Application.Current.Properties)
            {
                var _prop = new JObject();
                _prop.Add("type", new JValue(property.Value.GetType().ToString()));
                _prop.Add("value", JContainer.FromObject(property.Value, new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() }));

                obj.Add(property.Key as string, _prop);
            }


            using (var writer = new StreamWriter(new FileStream(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/settings.json", FileMode.Create)))
            {
                writer.Write(obj.ToString(Formatting.Indented));
            }

            base.OnExit(e);
        }
    }
}
