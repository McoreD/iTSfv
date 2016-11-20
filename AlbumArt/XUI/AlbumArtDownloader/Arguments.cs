using System;
using System.Collections.Generic;
using System.Linq;

namespace AlbumArtDownloader
{
	/// <summary>
	/// Helper class for parsing command line arguments into a list of parameters
	/// </summary>
	public class Arguments : List<Parameter>
	{
		public Arguments(string[] args) : this(args, new string[0]) { }
		/// <param name="valuedParameters">A list of parameter names which must be followed by values.</param>
		public Arguments(string[] args, IEnumerable<string> valuedParameters)
		{
			//Parameter switches start with - or /, and apply to the next arg, unless the next arg is also a switch.
			//For example /param1 "Hello There" /param2 /param3 Fred is 3 parameters: param1 = "Hello There", param2 = "", param3 = "Fred"
			//Parameters can also be passed without switches preceding them, in which case they have no name, and are accessible only by index, not by name.
			//Paremeters whose name appears in valuedParameters are always follwed by a value, even if the next arg would otherwise be a switch.
			//For example /param1 /hello would be parsed as param1 = "/hello" if "param1" was in valuedParameters.
			string paramName = null;
			foreach (string arg in args)
			{
				if ((paramName == null || !valuedParameters.Contains(paramName, StringComparer.OrdinalIgnoreCase)) && //If there is an existing parameter, and it is a Valued parameter, then don't treat it as a switch
					arg.Length > 0 && (arg[0] == '-' || arg[0] == '/')) //This is a switch
				{
					if (paramName != null)
					{
						//If there is an existing parameter, then this closes that parameter
						AddParameter(paramName); //The parameter has no value, though.
					}
					paramName = arg.Substring(1); //Strip off the switch character, and use this as the next name
				}
				else //This is a value
				{
					AddParameter(paramName, arg); //Add it as a parameter with value
					paramName = null; //This parameter name has been used up.
				}
			}
			if (paramName != null)
			{
				//If there a remaining parameter, close it with no value.
				AddParameter(paramName);
			}
		}

		private void AddParameter(string name)
		{
			AddParameter(name, "");
		}
		private void AddParameter(string name, string value)
		{
			this.Add(new Parameter(name, value));
		}

		/// <summary>
		/// Gets the value of a named parameter by name. If there are more
		/// than one parameters with the same name, the first is returned.
		/// </summary>
		/// <param name="name">The name of the parameter whose value to return</param>
		/// <param name="result">The value of the parameter</param>
		/// <returns>True if the parameter existed, False if it did not.</returns>
		public bool TryGetParameterValue(string name, out string result)
		{
			foreach (Parameter parameter in this)
			{
				if (parameter.Name == name)
				{
					result = parameter.Value;
					return true;
				}
			}
			result = null;
			return false;
		}

		/// <summary>
		/// Checks whether the named parameter exists or not.
		/// </summary>
		public bool Contains(string name)
		{
			string ignored;
			return TryGetParameterValue(name, out ignored);
		}
	}
	/// <summary>
	/// A parameter passed through the command line arguments
	/// </summary>
	public class Parameter
	{
		private string mName;
		private string mValue;
		
		public Parameter(string name, string value)
		{
			mName = name;
			mValue = value;
		}

		public string Name
		{
			get { return mName; }
		}
		public string Value
		{
			get { return mValue; }
			internal set { mValue = value; }
		}
	}
}