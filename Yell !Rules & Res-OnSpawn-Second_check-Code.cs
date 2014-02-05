if (limit.Activations(player.Name) > 1) return false;

plugin.Log("Logs/InsaneLimits/GUID5-7-13.log", plugin.R("[%date% %time%] [%p_ct% - %p_n%]    >>EA GUID:    %p_eg%<<         and         >>PB GUID:    %p_pg%<<     and      >>IP:    %p_ip%<<"));
//The thread code below allows me to delay the reward message on first spawn because directly below this text is a !rules yell. It then yells the reward message 5 seconds later

//simple yell to start for type !rules for server rules
plugin.SendPlayerMessage(player.Name, "Type !rules for Server Rules");

// Closure bindings for the delegate
string port = server.Port;
string host = server.Host;
string yellMsg = null;
string dir = "Plugins\\BF4\\ReserveList_" +host+ "_" +port+ ".txt";
if(File.Exists(dir))
  {
  //read names on reserve list
  string namecheck = File.ReadAllText(dir);
  //plugin.ConsoleWrite(namecheck);
  //split names on Reserve list at ", "
  string[] resnames = Regex.Split(namecheck, ", ");
  //check each split name 
  foreach (string resname in resnames)
    {
    //split the name at ":" in to array, 1st is player name, 2nd is date last updated, 3rd can be ignored, was extra code in second check
    string[] rescount = resname.Split(':');
    if (rescount[0] == player.Name)
      {
      if (plugin.GetReservedSlotsList().Contains(rescount[0]))
        {
        int value = Convert.ToInt32(rescount[2]);
        yellMsg = "You have a reserve slot for helping to start the server, it will expire in approximately "+value+" day(s) unless you help again.";
        break;
        }
      }
    }
  }

// Thread delegate

ThreadStart AdminYell = delegate  {
//5 second delay before yelling the reward message
  Thread.Sleep(5*1000);
  plugin.SendPlayerYell(player.Name, yellMsg, 5);
  };

// Main thread code
if (yellMsg != null)
  {
  Thread t = new Thread(AdminYell);
  t.Start();
  Thread.Sleep(10);
  }
return false;