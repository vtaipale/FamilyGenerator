using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyMember : MonoBehaviour
{
	public int charnumber = 1;
	public int dynasty = 1;
	public string dynastystring = "D'Hwanta";
	public string charname = "Ketil";
	public string charextranames = "";
	
	public int Importance = 15;
	public int Generation = 2;
	public int Prestige = 0;
	public float Fertility = 1.0f;
	public string Motivation = "";
	
	public int FatePoints = 0;
	public string religion = "imperial";
	public string culture = "voidborn";
	public bool female = false; //this is due how CK2 handles sex
	
	public FamilyMember father;
	public FamilyMember mother;
	public FamilyMember spouse;
	public int marriageyear = 0;
	
	public string Notes = "";

	public List<string>	traits = new List<string>();
	
	public int birth = 500; //m41
	public int death= 9999; //to mark someone alive
	public string deathreason = "no";
	public FamilyMember murderer;
	public bool living = true;
	public bool processed = false;
	public bool historic = false; //If do any processing
	
	public List<FamilyMember>	kills = new List<FamilyMember>();

	public List<FamilyMember>	MyKids = new List<FamilyMember>();
	
    // Start is called before the first frame update
    void Awake()
    {
		this.name = GetFullName();
		
	}
	
    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void addTrait(string newtrait)
	{
		if (traits.Contains(newtrait) == false)
			traits.Add(newtrait);
	}
	
	public string toString()
	{
		string TheString = charnumber + " = { \n";
		
		TheString += "  name=\"" + this.GetFirstNames() + "\" \n";
		TheString += "  dynasty= " + (5000000+dynasty) + " # " + dynastystring + " \n";
		TheString += "  religion=\"" + religion + "\" \n";
		TheString += "  culture=\"" + culture + "\" \n";

		if (this.female == true)
			TheString += "  female=yes \n";
		
		TheString += "\n  disallow_random_traits = yes  \n";
		TheString += "  historical = yes  \n";
		
		if (this.father != null)
			TheString += "  father=" + this.father.charnumber + " # " + father.GetFullName() + " \n";
		if (this.mother != null)
			TheString += "  mother=" + this.mother.charnumber + " # " + mother.GetFullName() + " \n";
		
		TheString += "  #Generation =" + this.Generation + " \n";
		TheString += "  #Importance =" + this.Importance + " \n";
		TheString += "  #Prestige   ="   + this.Prestige + " \n";
		TheString += "  #Fertility  = " + this.Fertility + " \n";
		if (this.Motivation != "")
			TheString += "  #Motivation ="   + this.Motivation + " \n";


		if (this.Notes != "")
			TheString += "  #Note: "   + this.Notes + " \n";


		foreach (string traitstring in traits)
			TheString += "\n  trait= \"" + traitstring + "\" ";
		
		if (this.kills.Count > 0)
		{
			TheString += "\n";
			foreach (FamilyMember victim in kills)
				TheString += "\n  #killed = \"" + victim.PersonHistoricalNote() + "  "+ victim.deathreason+"\" ";
		}
		
		
		TheString += "\n\n  " + birth + ".1.1 = { \n   birth = yes \n  }\n";
	
		if (this.marriageyear > 0 && spouse != null)
		{
		
		TheString += "\n  " + marriageyear + ".1.1 = { \n   add_spouse = "+ spouse.charnumber +" # " + spouse.GetFullName() + " \n   } \n";

		}
		if (this.living == false)
		{
			if (deathreason == "murder")
			{
				TheString += "  " + death + ".1.1 = { \n  death = {death_reason = death_murder \n ";
			
				if (this.murderer != null)
				{
					TheString += "   killer = " + murderer.charnumber + " # " + murderer.GetFullName() + " \n ";
				}
				TheString += "  } } ";
			}				
			else if (deathreason == "duel")
							{
				TheString += "  " + death + ".1.1 = { \n  death = {death_reason = death_duel \n ";
			
				if (this.murderer != null)
				{
					TheString += "   killer = " + murderer.charnumber + " # " + murderer.GetFullName() + " \n ";
				}
				TheString += "  } } ";
			}
			else if (deathreason == "dungeon")
							{
				TheString += "  " + death + ".1.1 = { \n  death = {death_reason = death_dungeon \n ";
			
				if (this.murderer != null)
				{
					TheString += "   killer = " + murderer.charnumber + " # " + murderer.GetFullName() + " \n ";
				}
				TheString += "  } } ";
			}	
			else if (deathreason == "execution")
							{
				TheString += "  " + death + ".1.1 = { \n  death = {death_reason = death_execution \n ";
			
				if (this.murderer != null)
				{
					TheString += "   killer = " + murderer.charnumber + " # " + murderer.GetFullName() + " \n ";
				}
				TheString += "  } } ";
			}			
			else if (deathreason != "yes")
				TheString += "  " + death + ".1.1 = { \n   death = { death_reason = "+deathreason+" } \n  }";
			else
				TheString += "  " + death + ".1.1 = { \n   death = yes \n  }"; //different death types later
		}
		TheString += "\n} ";

		return TheString;
	}
	public string GetFirstNames()
	{
		string TheString = "";

		if ((this.charextranames != null) && (this.charextranames.Length > 0))
			TheString += charname + " " +charextranames;
		else
			TheString += charname;
				
		return TheString;
	}
	public string GetFullName()
	{
		return GetFirstNames() + " " + dynastystring;
	}
	
	public int getAge(int CurrentYear)
	{
		if (this.living == true)
			return CurrentYear - birth;
		
		return death - birth;
	}
		
	public string GetLivingYears()
	{
		string returnoitava = "( " + this.birth + " -";
		
		if (this.living == false)
			returnoitava += " " + this.death + " )";
		else
			returnoitava += "-> )";
		
		return returnoitava;
	}

	public string PersonHistoricalNote()
	{
		return this.GetFullName() + " " +this.GetLivingYears();
	}
	
	public string PersonHistoricalNoteLong()
	{
		string returnoitava = this.GetFullName() + " " +this.GetLivingYears();
		returnoitava += " #I:" + this.Importance + " #P:"+ this.Prestige + " ";
		foreach (string traitstring in traits)
			returnoitava += " " + traitstring;
		
		return returnoitava;
	}
	public string PersonHistoricalNoteCommaSeparatedValues()
	{
		string returnoitava = "";
		returnoitava += this.charnumber +";";
		returnoitava += this.charname +";";
		returnoitava += this.charextranames +";";
		returnoitava += this.dynasty +";";
		returnoitava += this.dynastystring +";";
		returnoitava += this.culture +";";
		returnoitava += this.birth +";";
		returnoitava += this.death +";";
		returnoitava += this.deathreason +";";
		returnoitava += this.Motivation +";";
		returnoitava += this.Generation +";";
		returnoitava += this.Importance +";";
		returnoitava += this.Prestige +";";
		returnoitava += this.kills.Count +";";
		foreach (string traitstring in traits)
			returnoitava += "t:"+traitstring +" ";
		returnoitava += ";";
		returnoitava += this.getImportantParent().charnumber +";";
		if (marriageyear > 0)
			returnoitava += this.marriageyear +";";
		else 
			returnoitava += ";";
		if (this.spouse != null)
			returnoitava += this.spouse.charnumber +";";
		else if (marriageyear > 0)
			returnoitava += "outside;";
		else 
			returnoitava += ";";
		

		return returnoitava;
	}
	
	public int getParentAmount()
	{
		int returnoitava = 0;
		if (father != null)
			returnoitava++;
		if (mother != null)
			returnoitava++;
		
		return returnoitava;
	}
	
	public FamilyMember getImportantParent()
	{
		return this.transform.parent.GetComponent<FamilyMember>();
	}
	
	
	public int CheckAncestorsForName()
	{
		int amount = 0;
		
		foreach (FamilyMember ancestor in this.GetComponentsInParent<FamilyMember>())
		{
			if (ancestor.charname.Contains(this.charname))
				amount++;
		}
		//if (amount > 0)
			//Debug.Log("AncesttorNameCheck = " + amount);
			
		return amount;
	}
	
	
	public void Die()
	{
		this.deathreason="yes";
		this.living = false;
		this.processed = true;
		this.name = "D!"+this.getAge(death)+"-" + GetFullName();

	}
	
	public void Die(string DeathReasony)
	{
		this.Die();
		this.deathreason=DeathReasony;
		this.name = "D!"+this.getAge(death)+"-" + GetFullName();
	}
}
