List<PlayerInfoInterface> players = new List<PlayerInfoInterface>();
players.AddRange(team1.players);
players.AddRange(team2.players);
if (team3.players.Count > 0) players.AddRange(team3.players);
if (team4.players.Count > 0) players.AddRange(team4.players);

int RSThresh = 5;    //When the player should get a reserve slot, when he has this number of days or more.
int numoftemps = 10;  //Number of players to be copied to reserve list
int triggertransfer = 20;  //When the players list is copied over
int RSCap = 30;      //Max number of reserve slot days
int rewarddays = 5;  //Number of days rewarded for helping to start the server once
int totaltcount = server.PlayerCount;
int i = 0;
string port = server.Port;
string host = server.Host;
string dir = "Plugins\\BF4\\TempList_" +host+ "_" +port+ ".txt";
string done = "Plugins\\BF4\\Done_" +host+ "_" +port+ ".txt";
string logdir = "Logs\\InsaneLimits\\ReserveList_" +host+ "_" +port+ "dump_file.txt";

List<String> listofplayers = new List<String>();

foreach (PlayerInfoInterface p in players)
  {
  listofplayers.Add(p.Name);
  }


//check if server population is more than 4
if ((totaltcount >= 4) && (totaltcount <= triggertransfer) && (!File.Exists(done)))return true;
//if server population is less than 4 then delete temp file if it exists
else if(totaltcount < 4)
  {
  if (File.Exists(dir)) File.Delete(dir);
  if (File.Exists(done)) File.Delete(done);
  }
//check if server population is more than 10 then transfer to ReserveList
else if((totaltcount > triggertransfer) && (File.Exists(dir)))
  {

  int runcount = 0;

  if(File.Exists(dir))
    {
    //read names on temp list
    string namecheck = File.ReadAllText(dir);
    //split names on temp list at ", "
    string[] tempnames = Regex.Split(namecheck, ", ");
    //check each split name and add to reserve slot list
    foreach (string tempname in tempnames)
      {
      //split the name at ":" in to array, 1st is player name, 2nd is date last updated
      string[] tempcount = tempname.Split(':');
      if ((listofplayers.Contains(tempcount[0]) && runcount < numoftemps))
        {runcount++;
        plugin.ConsoleWrite(tempname);
        DateTime now = DateTime.Now;
        string datestring = now.ToString("d");

        string dir2 = "Plugins\\BF4\\ReserveList_" +host+ "_" +port+ ".txt";
        if(File.Exists(dir2))
          {
          //read names on reserve list
          string resnamecheck = File.ReadAllText(dir2);
          plugin.ConsoleWrite(resnamecheck);
          //split names on temp list at ", "
          string[] resnames = Regex.Split(resnamecheck, ", ");
          //finds last entry
          string reslastitem = resnames[resnames.Length - 1];
          //check each split name that is in the reserve list and updates if it is in the temp list
          foreach (string resname in resnames)
            {
            //split the reserve name at ":" in to array, 1st is player name, 2nd is the last date participated in seeding, 3rd is the number of days reward if he is on the reserve list, 4th is a date used for expiry
            //the 4th entry is used to subtract 1 digit a day off of the 3rd entry
            string[] rescount = resname.Split(':');
              //checks if the reserve name equals the player name in temp
            if (rescount[0] == tempcount[0])
              {
              //stops multiple rewards per day
              if(rescount[1] != datestring)
                {
                //add on the reward
                int value = Convert.ToInt32(rescount[2]);
                value = value + rewarddays;
                //cap the reserve slot days at RSCap
                if (value > RSCap) value = RSCap;
                //save the new data
                string newlist = resnamecheck.Replace(resname, rescount[0]+":"+ datestring +":"+value.ToString()+":"+rescount[3]);
                File.WriteAllText(dir2, newlist);
                //adds player to reserve slot if the have helped enough and are not on it.
                if (value >= RSThresh)
                  {
                  if (!plugin.GetReservedSlotsList().Contains(rescount[0]))
                    {
                    plugin.ServerCommand("reservedSlotsList.add", rescount[0]);
                    plugin.ServerCommand("reservedSlotsList.save");
                    if (plugin.GetReservedSlotsList().Contains(rescount[0]))
                      {
                      plugin.PRoConChat(rescount[0] + " got added to ReserveSlot successfully with " + value.ToString() + " day(s) remaining.");
                      plugin.Log(logdir, rescount[0] + " got added to the ReserveSlot successfully with " + value.ToString() + " day(s) remaining.");
                      }
                    else 
                      {
                      plugin.PRoConChat(rescount[0] + " failed to get added to the ReserveSlot.");
                      plugin.Log(logdir, rescount[0] + " failed to get added to the ReserveSlot.");
                      }
                    }
                  //message player
                  plugin.SendPlayerMessage(rescount[0], rescount[0] + ": You have been awarded a reserve slot for helping to start the server, it will expire in approximately "+value+" days unless you help again.");
                  }
                }
              //notify you that a player tried to help twice or more on a day to start server 
              else {plugin.ConsoleWrite(rescount[0] + " helped on the same day with no second reward.");}
              break;
              } 
            //if not in reserve list adds player and info to end of list.
            else if (resname == reslastitem)
              {
              string newlist = resnamecheck + ", " + tempcount[0] +":" + datestring +":"+ rewarddays +":"+ datestring;
              File.WriteAllText(dir2, newlist);
              plugin.PRoConChat("New Player," + tempcount[0] + "added to end of the reserve slot list.");
              plugin.Log(logdir, "New Player," + tempcount[0] + "added to end of the reserve slot list with " + rewarddays.ToString() + " day(s) remaining.");
              }
            }
          }
        //if reserve list doesnt exist, creates list with first entry
        else 
          {
          string newlist = "Blank, "+tempcount[0] +":"+ datestring +":"+ rewarddays +":"+ datestring;
          File.WriteAllText(dir2, newlist);
          plugin.PRoConChat("Reserve slot list created with first player," + tempcount[0] + ".");
          plugin.Log(logdir, "Reserve slot list created with first player," + tempcount[0] + ".");
          }
        }
      else if (runcount >= numoftemps)break;
      }
    }
//deletes temp list after transfer to reserve list
  File.Delete(dir);
  File.WriteAllText(done, "DONE");
  }

return false;