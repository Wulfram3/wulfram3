using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// KGF logic analyzer.
/// </summary>
public class KGFLogicAnalyzer
{
	public class KGFLogicOperand
	{
		public string itsOperandName = string.Empty;
		private bool? itsValue = null;
		public List<KGFLogicOperand> itsListOfOperands = new List<KGFLogicOperand> ();
		public List<string> itsListOfOperators = new List<string> ();

		public void AddOperand (KGFLogicOperand theOperand)
		{
			itsListOfOperands.Add (theOperand);
		}

		public void AddOperator (string theOperator)
		{
			itsListOfOperators.Add (theOperator);
		}

		public void SetName (string theName)
		{
			itsOperandName = theName;
			if (theName.ToLower () == "true")
			{
				itsValue = true;
			}
			else if (theName.ToLower () == "false")
			{
				itsValue = false;
			}
		}

		public string GetName ()
		{
			return itsOperandName;
		}

		public void SetValue (bool theValue)
		{
			itsValue = theValue;
		}

		public bool? GetValue ()
		{
			if (itsValue == null)
			{
				if (itsOperandName != string.Empty)
				{
					itsValue = KGFLogicAnalyzer.GetOperandValue (itsOperandName);
					if (itsValue == null)
					{
						return null;
					}
					else
					{
						return itsValue;
					}
				}
				else
				{
					return Evaluate ();
				}
			}
			else
			{
				return itsValue.Value;
			}
		}

		/// <summary>
		/// Evaluate the value of this operand. The value will be the result of the evaluations of all values and child operand evaluations.
		/// </summary>
		public bool? Evaluate ()
		{
			if (itsListOfOperands.Count == 1)
			{
				return itsListOfOperands [0].GetValue ();
			}
			else
			{
				bool? aCurrentValue = false;
				for (int i = 0; i< itsListOfOperands.Count-1; i++)
				{
					if (i == 0)
					{
						aCurrentValue = EveluateTwoOperands (itsListOfOperands [i].GetValue (), itsListOfOperands [i + 1].GetValue (), itsListOfOperators [i]);
					}
					else
					{
						aCurrentValue = EveluateTwoOperands (aCurrentValue, itsListOfOperands [i + 1].GetValue (), itsListOfOperators [i]);
					}
				}
				return aCurrentValue;
			}
		}

		/// <summary>
		/// Eveluates the result of two operands connected by an operator
		/// </summary>
		/// <returns>
		/// The two operands.
		/// </returns>
		/// <param name='theValue1'>
		/// The value1.
		/// </param>
		/// <param name='theValue2'>
		/// The value2.
		/// </param>
		/// <param name='theOperator'>
		/// The operator.
		/// </param>
		private bool? EveluateTwoOperands (bool? theValue1, bool? theValue2, string theOperator)
		{
			if (theValue1 == null)
			{
				Debug.LogError("KGFLogicAnalyzer: cannot evaluate because theValue1 is null");
				return null;
			}
			if (theValue2 == null)
			{
				Debug.LogError("KGFLogicAnalyzer: cannot evaluate because theValue2 is null");
				return null;
			}

			if (theOperator == "&&")
			{
				return theValue1.Value && theValue2.Value;
			}
			else if (theOperator == "||")
			{
				return theValue1.Value || theValue2.Value;
			}
			Debug.LogError("KGFLogicAnalyzer: wrong operator: " + theOperator);
			return null;
		}
	}

	private static string itsStringAnd = "&&";
	private static string itsStringOr = "||";
//	private static string itsStringBraceOpen = "(";
//	private static string itsStringBraceClose = ")";
	private static Dictionary<string,bool> itsOperandValues = new Dictionary<string, bool> ();

	/// <summary>
	/// Analyzes the specified theLogicString and returns true or false as a result.
	/// </summary>
	/// <param name='theLogicString'>
	/// The logic string.
	/// </param>
	public static bool? Analyze (string theLogicString)
	{
		string anErrorString = "";
//		bool? aValue = null;
		
		if (CheckSyntax (theLogicString, out anErrorString))
		{
			if (CheckOperands(theLogicString,out anErrorString))
			{
				int i = 0;
				if(!theLogicString.Contains(")"))
				{
					theLogicString = "("+theLogicString+")";
				}
				while (theLogicString.Contains(")"))
				{
					EvaluateBraces (ref theLogicString);
					i++;
					if (i == 30)
					{	//emergency exit
						break;
					}
				}
				if(theLogicString.ToLower() == "true")
				{
					return true;
				}
				else if(theLogicString.ToLower() == "false")
				{
					return false;
				}
				else
				{
					Debug.LogError("KGFLogicAnalyzer: unexpected result: "+theLogicString);
					return null;
				}
			}
			else
			{
				Debug.LogError("KGFLogicAnalyzer: syntax error: "+anErrorString);
				return null;
			}
		}
		else
		{
			Debug.LogError("KGFLogicAnalyzer: syntax error: "+anErrorString);
			return null;
		}
	}

	/// <summary>
	/// Evaluates the braces.
	/// </summary>
	/// <param name='aLogicString'>
	/// A logic string.
	/// </param>
	private static void EvaluateBraces (ref string theLogicString)
	{
		string aTrimmedLogicString = theLogicString.Replace (" ", "");
		int anIndexOfFirstClosingBrace = aTrimmedLogicString.IndexOf (')');
		string aSubString = aTrimmedLogicString.Substring (0, anIndexOfFirstClosingBrace + 1);
		int anIndexOfMatchingOpeningBrace = aSubString.LastIndexOf ('(');
		int aLengthOfLogicBlock = anIndexOfFirstClosingBrace - anIndexOfMatchingOpeningBrace - 1;
		string aLogicBlock = aTrimmedLogicString.Substring (anIndexOfMatchingOpeningBrace + 1, aLengthOfLogicBlock);
		bool? aLogicBlockResult = AnalyseLogicBlock (aLogicBlock);
		if (aLogicBlockResult == null)
		{
			Debug.LogError ("Logic block result is null. Something went wrong!");
			return;
		}
		else
		{
			string aStringBeforeOpeningBrace = theLogicString.Substring (0, anIndexOfMatchingOpeningBrace);
			string aStringAfterClosingBrace = theLogicString.Substring (anIndexOfFirstClosingBrace + 1);
			theLogicString = aStringBeforeOpeningBrace + aLogicBlockResult.Value.ToString () + aStringAfterClosingBrace;
		}
	}

	/// <summary>
	/// Clears the operand values. Invoke this method before setting new values and calling Analyze
	/// </summary>
	public static void ClearOperandValues ()
	{
		itsOperandValues.Clear();
	}

	/// <summary>
	/// Sets a predefined value for a named operand
	/// </summary>
	/// <param name='theOperandName'>
	/// The operand name.
	/// </param>
	/// <param name='theValue'>
	/// The value.
	/// </param>
	public static void SetOperandValue (string theOperandName, bool theValue)
	{
		if (itsOperandValues.ContainsKey (theOperandName))
		{
			itsOperandValues [theOperandName] = theValue;
		}
		else
		{
			itsOperandValues.Add (theOperandName, theValue);
		}
	}

	/// <summary>
	/// Gets the predefined value for a named operand
	/// </summary>
	/// <returns>
	/// The operand value.
	/// </returns>
	/// <param name='theOperandName'>
	/// If set to <c>true</c> the operand name.
	/// </param>
	public static bool? GetOperandValue (string theOperandName)
	{
		if (itsOperandValues.ContainsKey (theOperandName))
		{
			return itsOperandValues [theOperandName];
		}
		else
		{
			Debug.LogError ("KGFLogicAnalyzer: no operand value for operand: " + theOperandName);
			return null;
		}
	}

	/// <summary>
	/// Analyses the logic.
	/// </summary>
	/// <param name='theLogicString'>
	/// The logic string.
	/// </param>
	/// <param name='theOperand'>
	/// The operand.
	/// </param>
	private static bool? AnalyseLogicBlock (string theLogicString)
	{
		KGFLogicOperand aResultingOperand = new KGFLogicOperand ();
		string aTrimmedLogicString = theLogicString.Replace (" ", "");

		string[] aSeparators =
		{
			itsStringAnd,
			itsStringOr/*, itsStringEqual, itsStringNotEqual*/
		};
		string[] anOperandNames = aTrimmedLogicString.Split (aSeparators, System.StringSplitOptions.None);

		foreach (string anOperandName in anOperandNames)
		{
			KGFLogicOperand anOperand = new KGFLogicOperand ();
			anOperand.SetName (anOperandName);
			aResultingOperand.AddOperand (anOperand);
		}

		for (int i = 0; i< anOperandNames.Length-1; i++)
		{
			aTrimmedLogicString = aTrimmedLogicString.Remove (0, anOperandNames [i].Length);
			string anOperator = aTrimmedLogicString.Substring (0, 2);
			aResultingOperand.AddOperator (anOperator);
			aTrimmedLogicString = aTrimmedLogicString.Remove (0, 2);
		}
		return aResultingOperand.Evaluate ();
	}

	/// <summary>
	/// Checks the syntax of the logic string.
	/// </summary>
	/// <returns>
	/// The syntax.
	/// </returns>
	/// <param name='theLogicString'>
	/// If set to <c>true</c> the logic string.
	/// </param>
	public static bool CheckSyntax (string theLogicString, out string theErrorString)
	{
		theErrorString = "";
		string aLogicString = theLogicString;

		if (aLogicString.IndexOf (itsStringAnd) == 0)
		{
			theErrorString = "condition cannot start with &&";
			return false;
		}

		if (aLogicString.IndexOf (itsStringOr) == 0)
		{
			theErrorString = "condition cannot start with ||";
			return false;
		}

		if (aLogicString.LastIndexOf (itsStringAnd) == aLogicString.Length - 2 && aLogicString.Length != 1)
		{
			theErrorString = "condition cannot end with &&";
			return false;
		}

		if (aLogicString.LastIndexOf (itsStringOr) == aLogicString.Length - 2 && aLogicString.Length != 1)
		{
			theErrorString = "condition cannot end with ||";
			return false;
		}

		string aTrimmedLogicString = aLogicString.Replace (" ", "");

		int aNumberOfOpeningBraces = aTrimmedLogicString.Split ('(').Length - 1;
		int aNumberOfClosingBraces = aTrimmedLogicString.Split (')').Length - 1;

		if (aNumberOfOpeningBraces > aNumberOfClosingBraces)
		{
			theErrorString = "missing closing brace";
			return false;
		}
		else if (aNumberOfClosingBraces > aNumberOfOpeningBraces)
		{
			theErrorString = "missing opening brace";
			return false;
		}

		string[] aSeparators =
		{
			itsStringAnd,
			itsStringOr/*, itsStringEqual, itsStringNotEqual*/
		};

		string aLogicStringWithoutBraces = aTrimmedLogicString.Replace("(","");
		aLogicStringWithoutBraces = aLogicStringWithoutBraces.Replace(")","");
		string[] anOperandNames = aLogicStringWithoutBraces.Split (aSeparators, System.StringSplitOptions.None);

		foreach (string anOperandName in anOperandNames)
		{
			if (anOperandName.Contains ("&"))
			{
				theErrorString = "condition cannot contain the character &. Use && for logical and.";
				return false;
			}
			if (anOperandName.Contains ("|"))
			{
				theErrorString = "condition cannot contain the character |. Use || for logical or.";
				return false;
			}
		}
		return true;
	}
	
	public static bool CheckOperands(string theLogicString,out string theErrorString)
	{
		theErrorString = "";
		string[] aSeparators =
		{
			itsStringAnd,
			itsStringOr/*, itsStringEqual, itsStringNotEqual*/
		};
		
		string aLogicString = theLogicString;
		string aTrimmedLogicString = aLogicString.Replace (" ", "");
		string aLogicStringWithoutBraces = aTrimmedLogicString.Replace("(","");
		aLogicStringWithoutBraces = aLogicStringWithoutBraces.Replace(")","");
		string[] anOperandNames = aLogicStringWithoutBraces.Split (aSeparators, System.StringSplitOptions.None);
		
		foreach (string anOperandName in anOperandNames)
		{
			bool? anOperandValue = GetOperandValue(anOperandName);
			if(anOperandValue == null)
			{
				theErrorString = "no operand value for operand: " + anOperandName;
				return false;
			}
		}
		return true;
	}
}
