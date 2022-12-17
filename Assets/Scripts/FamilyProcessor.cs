using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///Post-generation processing of the Family
public class FamilyProcessor : MonoBehaviour
{
	
	public FamilyMember Target;
		
	//Due Player Character Actions
	public void Tragedy()
	{
		
		int DynastyToSuffer = 9;
		
		int YearOfTragedy = 794;
		
		string TragedyReason = "accident";
		
		int charactersDied = 0;
		
		int unmadeCharacters = 0;
		
		int marriagesVoided = 0; 
				
		foreach (FamilyMember Person in FindObjectsOfType<FamilyMember>())
		{	
			if ((Person.dynasty == DynastyToSuffer) && (Person.birth > YearOfTragedy)) //neverborn
			{
				unmadeCharacters++;
				//person.GameObject.setActive(false);
				Person.name = "Nobody";
				Person.charname = "Nobody";
				Person.Notes = "Nonexistent due Temporal Error";
			}
			else if ((Person.dynasty == DynastyToSuffer) && (Person.death > YearOfTragedy))
			{
				charactersDied++;
				
				Person.death= YearOfTragedy;
				Person.deathreason = TragedyReason;
				Person.Notes = "Died in The Accident";

				if (Person.birth > YearOfTragedy-30) // too young
				Person.Motivation = "";
				
				if (Person.marriageyear > YearOfTragedy)
				{
					marriagesVoided++;
					
					Person.marriageyear = 0;
					
					if (Person.spouse != null)
					{
						Person.spouse.spouse = null;
						Person.spouse = null;
					}
				}
			}
		}
		Debug.Log("Accident done. " + charactersDied + " " + unmadeCharacters+ " " +marriagesVoided);
	
	}
	
}
