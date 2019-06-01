using System;

namespace KauRock {
	public static class Log {

		public enum Level {
			Debug = 0,
			Info = 1,
			Warning = 2,
			Error = 3,
			};

			// Print a message to the console. Example: '[Debug] message'
			public static void Print (Level level, string message, string source = null) {
			Console.Write ("[");
			Console.Write (level.ToString ());
			Console.Write ("] ");

			if (source != null) {
			Console.Write (source);
			Console.Write (": ");
			}

			Console.WriteLine (message);
		}

		public static void Debug (object from, string format, params object[] args) {

			string source = null;
			if (from != null)
				source = from.ToString ();

			Print (Level.Debug, string.Format (format, args), source);
		}

		public static void Debug (object from, object value) {

			string source = null;
			if (from != null)
				source = from.ToString ();

			Print (Level.Debug, value.ToString (), source);
		}

		public static void Debug (object from, string message) {

			string source = null;
			if (from != null)
				source = from.ToString ();

			Print (Level.Debug, message, source);
		}

		public static void Info (object from, string format, params object[] args) {

			string source = null;
			if (from != null)
				source = from.ToString ();

			Print (Level.Info, string.Format (format, args), source);
		}

		public static void Info (object from, object value) {

			string source = null;
			if (from != null)
				source = from.ToString ();

			Print (Level.Info, value.ToString (), source);
		}

		public static void Info (object from, string message) {

			string source = null;
			if (from != null)
				source = from.ToString ();

			Print (Level.Info, message, source);
		}

		public static void Warning (object from, string format, params object[] args) {

			string source = null;
			if (from != null)
				source = from.ToString ();

			Print (Level.Warning, string.Format (format, args), source);
		}

		public static void Warning (object from, object value) {

			string source = null;
			if (from != null)
				source = from.ToString ();

			Print (Level.Warning, value.ToString (), source);
		}

		public static void Warning (object from, string message) {

			string source = null;
			if (from != null)
				source = from.ToString ();

			Print (Level.Warning, message, source);
		}

		public static void Error (object from, string format, params object[] args) {

			string source = null;
			if (from != null)
				source = from.ToString ();

			Print (Level.Error, string.Format (format, args), source);
		}

		public static void Error (object from, object value) {

			string source = null;
			if (from != null)
				source = from.ToString ();

			Print (Level.Error, value.ToString (), source);
		}

		public static void Error (object from, string message) {

			string source = null;
			if (from != null)
				source = from.ToString ();

			Print (Level.Error, message, source);
		}

		public static void Error (Exception exception) {
			Print (Level.Error, exception.ToString (), null);
		}
	}
}