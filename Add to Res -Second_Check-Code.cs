List<PlayerInfoInterface> players = new List<PlayerInfoInterface>();
players.AddRange(team1.players);
players.AddRange(team2.players);
if (team3.players.Count > 0) players.AddRange(team3.players);
if (team4.players.Count > 0) players.AddRange(team4.players);

string port = server.Port;
string host = server.Host;

foreach (PlayerInfoInterface p in players)
  {
  // New tag extraction code by PapaCharlie9
  String tag = p.Tag;

    if (String.IsNullOrEmpty(tag) && !p.StatsError) {
        // Maybe they are using [_-=]XXX[=-_]PlayerName[_-=]XXX[=-_] format
        Match tm = Regex.Match(p.Name, @"^[=_\-]*([^=_\-]{2,4})[=_\-]");
        if (tm.Success) {
            tag = tm.Groups[1].Value;
        } else {
            tm = Regex.Match(p.Name, @"[^=_\-][=_\-]([^=_\-]{2,4})[=_\-]*$");
            if (tm.Success) { 
                tag = tm.Groups[1].Value;
            } else {
                tag = String.Empty;
            }
        }
    }

//This stops your clan members and any other clan friends in the "Res" list being added to the reward and them possibly later getting removed

  if ((!plugin.isInList(p.Name, "Res")) && (!plugin.isInList(tag, "Res")))
    {
    string dir = "Plugins\\BF4\\TempList_" +host+ "_" +port+ ".txt";
    if(File.Exists(dir))
      {
      string namecheck = File.ReadAllText(dir);
//    plugin.ConsoleWrite(namecheck);
      string[] tempnames = Regex.Split(namecheck, ", ");
      string templastitem = tempnames[tempnames.Length - 1];
      //stops the names being repeatedly entered in to the list every time it runs
      foreach (string tempname in tempnames)
        {
        string[] tempcount = tempname.Split(':');
        if (tempcount[0] == p.Name)
          {
          break;
          }    
        else if (tempname == templastitem)
          {
          //add new player
          plugin.ConsoleWrite("Templist New Player: " + p.FullName);
          DateTime now = DateTime.Now;
          string datestring = now.ToString("d");
          string newlist = namecheck + ", " + p.Name +":" + datestring;
          File.WriteAllText(dir, newlist);
          }
        }
      }
    else
      {
      //create new temp file with player
      plugin.ConsoleWrite("Tempfile New Player: " + p.FullName);
      DateTime now = DateTime.Now;
      string datestring = now.ToString("d");
      string newlist = "Blank, "+p.Name +":"+ datestring;
      File.WriteAllText(dir, newlist);
      }
    }
  }
return false;