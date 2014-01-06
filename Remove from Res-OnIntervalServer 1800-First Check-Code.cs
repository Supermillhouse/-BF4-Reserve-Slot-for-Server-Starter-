int RSOffThresh = 0;   //Remove Reserve slot on number of days
int RSDellThresh = -1; //Remove from Reserve list on number of days
bool deletedlog = false;
string port = server.Port;
string host = server.Host;
string dir = "Plugins\\BF4\\ReserveList_" +host+ "_" +port+ ".txt";
string logdir = "Logs\\InsaneLimits\\ReserveList_" +host+ "_" +port+ "dump_file.txt";

if(File.Exists(dir))
  {
  string namecheck = File.ReadAllText(dir);
  //plugin.ConsoleWrite(namecheck);
  string[] resnames = Regex.Split(namecheck, ", ");
  DateTime now = DateTime.Now;
  string datestring = now.ToString("d");
  foreach (string resname in resnames)
    {
    if (resname != "Blank")
      {
      string[] rescount = resname.Split(':');
      if (rescount[3] != datestring)
        {
        if ((File.Exists(logdir)) && (deletedlog == false))
          {
          File.Delete(logdir);
          deletedlog = true;
          }
        int value = Convert.ToInt32(rescount[2]);
        value--;
        if ((value == RSOffThresh) && (plugin.GetReservedSlotsList().Contains(rescount[0])))
          {
          plugin.ServerCommand("reservedSlotsList.remove", rescount[0]);
          plugin.ServerCommand("reservedSlotsList.save");
          if (!plugin.GetReservedSlotsList().Contains(rescount[0]))
             {
             plugin.PRoConChat(rescount[0] + " got removed from the ReserveSlot successfully.");
             }
          else 
             {
             plugin.PRoConChat(rescount[0] + " failed to get removed from the ReserveSlot.");
             }
          //plugin.ConsoleWrite(value.ToString());
          namecheck = namecheck.Replace(resname, rescount[0]+":"+ rescount[1] +":"+value.ToString()+":"+ datestring);
          plugin.ConsoleWrite(namecheck);
          File.WriteAllText(dir, namecheck);
          }
        else if(value <= RSDellThresh)
          {
          plugin.ConsoleWrite(value.ToString());
          namecheck = namecheck.Replace(", "+resname, "");
          plugin.ConsoleWrite(namecheck);
          File.WriteAllText(dir, namecheck);
          }
        else
          {
          plugin.ConsoleWrite(value.ToString());
          namecheck = namecheck.Replace(resname, rescount[0]+":"+ rescount[1] +":"+value.ToString() +":"+ datestring);
          plugin.ConsoleWrite(namecheck);
          File.WriteAllText(dir, namecheck);
          }
        if (plugin.GetReservedSlotsList().Contains(rescount[0]))
          {
         plugin.Log(logdir, rescount[0] + " has " + value.ToString() + " day(s) left in the ReserveSlot list.");
          }
        else
          {
         plugin.Log(logdir, rescount[0] + " has " + value.ToString() + " day(s) left until he exists no more as he/she is a 2nd time no show.");
          }
        }
      }
    }
  }
return false;