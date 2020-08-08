using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using KauRock;
using System.Threading.Tasks;

namespace KauRock {
  public delegate string Command (params string[] args);
  public static class CommandManager {

    private static Dictionary<string, Command> commands = new Dictionary<string, Command>();

    public static void Add (string command, Command action) => commands.Add( command, action );
    public static bool Exists (string command) => commands.ContainsKey( command );
    public static void Remove (string command) => commands.Remove( command );

    private static Thread LineReader;
    private static Action<string> inputReceived = InputReceived;

    private static void InputReceived (string obj) {
      var result = Execute(obj);
      if(!String.IsNullOrWhiteSpace(result))
        Console.WriteLine(result);
    }

    public static void SetStdIn (System.IO.TextReader stdIn) {
      if ( LineReader != null )
        LineReader.Abort();

      LineReader = new Thread( InputLine );
      LineReader.Start( stdIn );
    }

    public static void InputLine (object obj) {
      var input = ( System.IO.TextReader ) obj;
      while (true) {
        var command = input.ReadLine();

        if(command != null && inputReceived != null)
          inputReceived.Invoke(command);
      }
    }

    public static string Execute (string input) {

      if ( string.IsNullOrWhiteSpace( input ) )
        return "";


      // Split up the string at every space while honouring quote marks.
      var collection = Regex.Matches( input, "(\"[^\"]+\"|[^\\s\"]+)", RegexOptions.Compiled );

      // Create an array for the arguments.
      string[] args;
      if ( collection.Count > 1 )
        args = new string[collection.Count - 1];
      else
        args = new string[0];

      // Get the command and assign each arg (if any exist)
      string command = null;
      int count = 0;
      foreach ( Match match in collection ) {

        // Set the command from the first 'argument'
        if ( command == null )
          command = match.Value;
        else {
          args[count] = match.Value;
          count++;
        }
      }

      // Execute the command.
      return Execute( command, args );
    }
    public static string Execute (string command, params string[] args) {
      if ( commands.ContainsKey( command ) )
        commands[command].Invoke( args );

      return string.Format( "{0} was not found. Use help for a list of commands ", command );
    }
  }

  [System.AttributeUsage( System.AttributeTargets.Method )]
  public class CommandInfo : System.Attribute {
    private string helpText;

    public CommandInfo (string helpText) {
      this.helpText = helpText;
    }
  }
}