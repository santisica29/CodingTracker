using System.Configuration;

string sAttr;
sAttr = ConfigurationManager.AppSettings["ConnectionString"];
Console.WriteLine("The value of Key0 is " + sAttr);

