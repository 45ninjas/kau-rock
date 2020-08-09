using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using KauRock;
using System.Reflection;

namespace KauRock {
  public class Commands : Component {
    public delegate string Command (params string[] args);
    private Dictionary<string, Command> commands = new Dictionary<string, Command>();

    public void Add (string command, Command action) {
      commands.Add( command, action );
    }
    public bool Exists (string command) => commands.ContainsKey( command );
    public void Remove (string command) => commands.Remove( command );

    private LineReader lineReader;
    public override void OnEnabled () {
      lineReader = new LineReader( Console.In );
      lineReader.ReceivedLine += InputReceived;
      AddAttributes();
    }
    public override void OnDisabled () {
      OnDestroy();
    }
    public override void OnDestroy() {
      lineReader.ReceivedLine -= InputReceived;
      lineReader.Shutdown();
    }

    public void AddAttributes () {
      var assem = Assembly.GetExecutingAssembly();
      foreach ( var type in assem.GetTypes() ) {
        var attribs = type.GetCustomAttributes<KauRock.Command>();
        foreach ( var attrib in attribs ) {
          Log.Debug( this, attrib );
        }
      }
    }

    private void InputReceived (string obj) {
      var result = Execute( obj );
      if ( !String.IsNullOrWhiteSpace( result ) )
        Log.Info( "Command Manager", result );
    }

    public string Execute (string input) {

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
    public string Execute (string command, params string[] args) {
      if ( commands.ContainsKey( command ) )
        commands[command].Invoke( args );

      return string.Format( "{0} was not found. Use help for a list of commands ", command );
    }


    public class LineReader {
      readonly System.IO.TextReader input;
      private Thread thread;
      private CancellationTokenSource cancel;

      public Action<string> ReceivedLine;

      public LineReader (System.IO.TextReader input) {
        if ( input == null )
          throw new NullReferenceException();

        this.input = input;
        cancel = new CancellationTokenSource();
        thread = new Thread( Main );
        thread.Start();
      }
      private void Main () {
        Log.Debug(this, "Thread Started");

        while ( !cancel.Token.IsCancellationRequested ) {
          var task = input.ReadLineAsync();
          if (task.IsCompletedSuccessfully && !string.IsNullOrWhiteSpace( task.Result ) && ReceivedLine != null) {
            ReceivedLine.Invoke( task.Result );
          }
        }
      }
      public void Shutdown () {
        Log.Debug(this, "Shutdown!");
        cancel.Cancel();
        thread.Join();
      }
    }
  }

  [System.AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
  public class Command : System.Attribute {
    private string helpText;
    private string command;

    public Command (string command, string helpText) {
      this.command = command;
      this.helpText = helpText;
    }
  }
}