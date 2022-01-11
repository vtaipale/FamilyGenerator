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
				returnoitava += addon + Person.GetFullName() + " " +GetLivingYears(Person)+"\n";
			else if (Person.spouse == null)
				returnoitava += addon + Person.GetFullName() + " " +GetLivingYears(Person)+" --- marr. " +Person.marriageyear+" ouside the Dynasty\n";
			else
				returnoitava += addon + Person.GetFullName() + " " +GetLivingYears(Person)+" --- marr. " +Person.marriageyear+ " " + Person.spouse.GetFullName() + " " +GetLivingYears(Person.spouse)+"\n";
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
				returnoitava += "  " +Person.GetFullName() + " " +GetLivingYears(Person)+"\n";
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
			
			if (nuBirths.Count > 0)
			{
				returnoitava += "\n There were " + nuBirths.Count + " newborns in the Dynasty:\n";
			
				foreach (FamilyMember Person in nuBirths)
				{
					returnoitava += "  " + Person.GetFullName() + " " +GetLivingYears(Person);
					
					if(Person.getParentAmount() > 0)
						returnoitava += " was born to " + Person.getImportantParent().GetFullName() + " " +GetLivingYears(Person.getImportantParent()) + ".\n";
					else 
						returnoitava += " was born.\n";
				}		
			}
			if (nuMarriages.Count > 0)
			{
				returnoitava += "\n There were " + nuMarriages.Count + " marriages in the Dynasty:\n";
			
				foreach (FamilyMember Person in nuMarriages)
				{
					returnoitava += "  " + Person.GetFullName() + " " +GetLivingYears(Person)+"\n   married ";
					
					if(Person.spouse == null)
						returnoitava += "outside the Dynasty.\n";
					else  //dublicates ? TODO fix!
					{
						returnoitava += Person.spouse.GetFullName() + " " +GetLivingYears(Person.spouse) +"!\n";
					}
				}		
			}
			if (nuDeaths.Count > 0)
			{
				returnoitava += "\n There were " + nuDeaths.Count + " deaths in the Dynasty:\n";
			
				foreach (FamilyMember Person in nuDeaths)
				{
					if (Person.deathreason == "yes")
						returnoitava += "  " + Person.GetFullName() + " " +GetLivingYears(Person)+"\n";
					else if (Person.murderer != null)
						returnoitava += "  " + Person.GetFullName() + " " +GetLivingYears(Person)+" " +Person.deathreason+ " by " + Person.murderer.GetFullName() + " " + GetLivingYears(Person.murderer) +"\n";
					else
						returnoitava += "  " + Person.GetFullName() + " " +GetLivingYears(Person)+" " +Person.deathreason+ "\n";

				}		
			}
			
			
			
		}
		
		returnoitava += "And Here the Chronicle ends...  \n\n"+ AnalyzeDescendants(FamilyRoot);
				
		TheHistory = returnoitava;
		
		return returnoitava;
	}
	
	private string GetLivingYears(FamilyMember Target)
	{
		string returnoitava = "( " + Target.birth + " -";
		
		if (Target.living == false)
			returnoitava += " " + Target.death + " )";
		else
			returnoitava += "-> )";
		
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
	
	
	public void EXPORT()
	{
		string returnoitava = "";
		returnoitava += "# SEED : " + Random.seed;
		
		returnoitava += this.WriteHistoryAndDescendants(FamilyRoot);

		returnoitava = returnoitava.Replace("\n", System.Environment.NewLine);

		int randomiser = Random.Range(0,1000);

		System.IO.File.WriteAllText(@"D:\temp\FamGenCharsExport" + randomiser + ".txt", returnoitava);

		Debug.Log("exported TO D: temp FamGenCharsExport" + randomiser + " !");

	}
	
}
