using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Server.Settings
{
    public class CommandSetting
    {
        public static List<CommandSetting> Load(string filename)
        {
            List<CommandSetting> settings = new List<CommandSetting>();
            try
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(filename);

                foreach (XmlNode node in xd.SelectNodes("/CommandSettings/CommandSetting"))
                {
                    try
                    {
                        CommandSetting setting = new CommandSetting();
                        setting.Name = node.Attributes["Name"].Value;
                        foreach (XmlNode commandNode in node.SelectNodes("Command"))
                        {
                            setting.Commands.Add(commandNode.InnerText);
                        }

                        if (setting.Commands.Count != 0) settings.Add(setting);
                    }
                    catch
                    {

                    }
                }

            }
            catch
            {

            }

            return settings;
        }

        public string Name { get; private set; } = "";

        public List<string> Commands { get; private set; } = new List<string>();

        private CommandSetting()
        {

        }


    }
}
