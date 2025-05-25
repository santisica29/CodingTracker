using System.Configuration;
using System.Collections.Specialized;

string sAttr;
sAttr = ConfigurationManager.AppSettings["Key0"];
Console.WriteLine("The value of Key0 is " + sAttr);

Console.WriteLine(ConfigurationManager.AppSettings["Key1"]);
