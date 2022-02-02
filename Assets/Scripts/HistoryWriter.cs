using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryWriter : MonoBehaviour
{
	
	public FamilyMember FamilyRoot;

	public string TheHistory = "";
	
	public FamilyGenerator FamilyGenner;
	
	public void WriteHistory(FamilyMember Target)
	{
		TheHistory = "#NuHistory";
		
		TheHistory += Target.toString();
		
	}
	
	public void b_WriteHistoryAndDescendants(FamilyMember Target)
	{
		this. WriteHistoryAndDescendants(Target);
	}
	
	public string WriteHistoryAndDescendants(FamilyMember Target)
	{
		string returnoitava = "#Life And Descendants of "+Target.GetFullName()+"\n\n";
		
		returnoitava +=AnalyzeDescendants(Target);
		
		
		//TheHistory += Target.toString();
		
		foreach (FamilyMember Person in Target.gameObject.GetComponentsInChildren<FamilyMember>())
		{
		returnoitava += Person.toString() + "\n\n";			
		}
		
		TheHistory = returnoitava;
		
		return returnoitava;
	}
		
	public void b_WriteHistoryAndDescendants_SHORT(FamilyMember Target)
	{
		this. WriteHistoryAndDescendants_SHORT(Target);
	}
	
	public string WriteHistoryAndDescendants_SHORT(FamilyMember Target)
	{
		string returnoitava = "#Life And Descendants of "+Target.GetFullName()+"\n\n";
				
		//TheHistory += Target.toString();
		
		foreach (FamilyMember Person in Target.gameObject.GetComponentsInChildren<FamilyMember>())
		{
			string addon ="";
			for (int index = 0; index < Person.Generation; index++)
				addon +=" ";
			
			if (Person.marriageyear == 0)
				returnoitava += addon + PersonHistoricalNote(Person)+"\n";
			else if (Person.spouse == null)
				returnoitava += addon + PersonHistoricalNote(Person)+" --- marr. " +Person.marriageyear+" ouside the Dynasty\n";
			else
				returnoitava += addon + PersonHistoricalNote(Person)+" --- marr. " +Person.marriageyear+ " " + PersonHistoricalNote(Person.spouse)+"\n";
		}
		
		TheHistory = returnoitava;
		
		return returnoitava;
	}
	
	public void b_WriteYearChronicle(int StartYear)
	{
		this. WriteYearChronicle(StartYear);
	}
	
	private string WriteYearChronicle(int StartYear)
	{
		float startTime = Time.time;
		
		int EndYear = FamilyGenner.currentYear;
		
		string returnoitava = "Yearly Chronicle for Descendants of "+ FamilyRoot.GetFullName()+ "\n\n--We begin in the year " + StartYear;
				
		
		List<FamilyMember> startingMembers = new List<FamilyMember>();
 		
		foreach (FamilyMember Person in FamilyRoot.gameObject.GetComponentsInChildren<FamilyMember>())
		{
			if ((Person. birth < StartYear) && (Person. death > StartYear)) {
				startingMembers.Add(Person);
			}
		}
		
		if (startingMembers.Count > 0)
		{
		returnoitava += "\n\n There were " + startingMembers.Count + " members in the Dynasty: \n";
			
			foreach (FamilyMember Person in startingMembers)
			{
				returnoitava += "  " +PersonHistoricalNote(Person)+"\n";
			}
		}
		
		//The Actual Year cycling
		
		List<FamilyMember>	nuBirths = new List<FamilyMember>();
		List<FamilyMember>	nuDeaths = new List<FamilyMember>();
		List<FamilyMember>	nuMarriages = new List<FamilyMember>();
		
		for (int index = StartYear; index <= EndYear; index++)
		{
			if (index > StartYear)
				returnoitava += "\n--Year " + index;


			nuBirths.Clear();
			nuDeaths.Clear();
			nuMarriages.Clear();

			foreach (FamilyMember Person in FamilyRoot.gameObject.GetComponentsInChildren<FamilyMember>())
			{
			if (Person. birth == index)
				nuBirths.Add(Person);
			else if (Person.marriageyear == index)
			{
				nuMarriages.Add(Person);
				
				if(Person.spouse != null)
					nuMarriages.Remove(Person.spouse);
			}
			if ((Person.death == index)&& (Person.living == false))
				nuDeaths.Add(Person);
			}
			
			
			//the string 
			
			if (nuBirths.Count > 0)
			{
				returnoitava += "\n There were " + nuBirths.Count + " newborns in the Dynasty:\n";
			
				foreach (FamilyMember Person in nuBirths)
				{
					returnoitava += "  " + PersonHistoricalNote(Person);
					
					if(Person.getParentAmount() > 0)
						returnoitava += " was born to " + PersonHistoricalNote(Person.getImportantParent())+ ".\n";
					else 
						returnoitava += " was born.\n";
				}		
			}
			if (nuMarriages.Count > 0)
			{
				returnoitava += "\n There were " + nuMarriages.Count + " marriages in the Dynasty:\n";
			
				foreach (FamilyMember Person in nuMarriages)
				{
					returnoitava += "  " + PersonHistoricalNote(Person)+"\n   married ";
					
					if(Person.spouse == null)
						returnoitava += "outside the Dynasty.\n";
					else  if (Person.female == true)//only women marriages noted, fix to dublicates
					{
						returnoitava += PersonHistoricalNote(Person.spouse) +"!\n";
					}
				}		
			}
			if (nuDeaths.Count > 0)
			{
				returnoitava += "\n There were " + nuDeaths.Count + " deaths in the Dynasty:\n";
			
				foreach (FamilyMember Person in nuDeaths)
				{
					if (Person.deathreason == "yes")
						returnoitava += "  " + PersonHistoricalNote(Person)+"\n";
					else if (Person.murderer != null)
						returnoitava += "  " + PersonHistoricalNote(Person)+" " +Person.deathreason+ " by " + PersonHistoricalNote(Person.murderer) +"\n";
					else
						returnoitava += "  " + PersonHistoricalNote(Person)+" " +Person.deathreason+ "\n";

				}		
			}
			
			
			
		}
		
		returnoitava += "And Here the Chronicle ends...  \n\n"+ AnalyzeDescendants(FamilyRoot);
				
		TheHistory = returnoitava;
		
		Debug.Log("Chronicle Written for " +FamilyRoot+ "!  Duration:" + (Time.time-startTime));
		
		return returnoitava;
	}

	private string AnalyzeDescendants(FamilyMember Target)
	{
		int AliveMembers = 0;
		int DeadMembers = 0;
		int DynastyChildren = 0;
		int Nobleborns = 0;
		int Inbreds = 0;
		
		foreach (FamilyMember Person in Target.gameObject.GetComponentsInChildren<FamilyMember>())
		{	
			if (Person.living == true)
				{
					AliveMembers++;
										
					if (Person.culture == "childOfDynasty"){
						DynastyChildren++;
						if (Person.traits.Contains("inbred"))
							Inbreds++;
					}
					else if (Person.culture == "noble")
						Nobleborns++;
				}
			else
			{
				DeadMembers++;
			}
		}
		
		
		return ("#Personal Descendants; \n #Alive: "+AliveMembers+" | Nobleborns: "+Nobleborns+" | DynastyChilds: "+DynastyChildren + " | Inbreds:" + Inbreds+" | Dead: "+DeadMembers + "\n\n");
		
		
	}

	public string PersonHistoricalNote(FamilyMember P)
	{
		return P.PersonHistoricalNote();
	}
	
	public void EXPORT()
	{		
		float startTime = Time.time;

		string returnoitava = "";
		returnoitava += "# SEED : " + Random.seed;
		
		returnoitava += this.WriteHistoryAndDescendants(FamilyRoot);

		returnoitava = returnoitava.Replace("\n", System.Environment.NewLine);
		
		Debug.Log("Export Text ready, next actual txt!  Duration:" + (Time.time-startTime));

		int randomiser = Random.Range(0,1000);

		System.IO.File.WriteAllText(@"D:\temp\FamGenCharsExport" + randomiser + ".txt", returnoitava);
		
		Debug.Log("EXPORT DONE!  Duration:" + (Time.time-startTime));

		Debug.Log("exported TO D: temp FamGenCharsExport" + randomiser + " !");

	}
	
}
