using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyMember : MonoBehaviour
{
	public int charnumber = 1;
	public int dynasty = 1;
	public string dynastystring = "D'Hwanta";
	public string charname = "Ketil";
	public string charextranames = "Egil";
	
	public int Importance = 15;
	public int Generation = 2;
	public int Prestige = 0;
	public int FatePoints = 0;
	public string religion = "imperial";
	public string culture = "voidborn";
	public bool female = false; //this is due how CK2 handles sex
	
	public FamilyMember father;
	public FamilyMember mother;
	public FamilyMember spouse;
	public int marriageyear = 0;
	
	public List<string>	traits = new List<string>();
	
	public int birth = 500; //m41
	public int death= 9999; //to mark someone alive
	public string deathreason = "no";
	public FamilyMember murderer;
	public bool living = true;
	public bool processed = false;
	public bool historic = false; //If do any processing
	
	public FamilyMember[] Descendants;
	
    // Start is called before the first frame update
    void Start()
    {
		this.name = GetFullName();
		
        //Debug.Log(this.toString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void addTrait(string newtrait)
	{
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
		
		TheString += "  #Generation " + this.Generation + " \n";
		TheString += "  #Importance " + this.Importance + " \n";
		TheString += "  #Prestige "   + this.Prestige + " \n";
		
		foreach (string traitstring in traits)
			TheString += "\n  trait= \"" + traitstring + "\" ";
		
		TheString += "\n\n  " + birth + ".1.1 = { \n   birth = yes \n  }\n";
	
		if (this.marriageyear > 0 && spouse != null)
		{
		
		TheString += "\n  " + marriageyear + ".1.1 = { \n   add_spouse = "+ spouse.charnumber +" # " + spouse.GetFullName() + " \n   } \n";

		}
		if (this.living == false)
		{
			if (deathreason == "death_murder")
			{
				TheString += "  " + death + ".1.1 = { \n  death = {death_reason = death_murder \n ";
			
				if (this.murderer != null)
				{
					TheString += "   killer = " + murderer.charnumber + " # " + murderer.GetFullName() + " \n ";
				}
				TheString += "  } } ";
			}				
			else if (deathreason == "death_duel")
							{
				TheString += "  " + death + ".1.1 = { \n  death = {death_reason = death_duel \n ";
			
				if (this.murderer != null)
				{
					TheString += "   killer = " + murderer.charnumber + " # " + murderer.GetFullName() + " \n ";
				}
				TheString += "  } } ";
			}	
			else if (deathreason == "death_murder_unknown")
				TheString += "  " + death + ".1.1 = { \n   death = { death_reason = death_murder_unknown } \n  }";
			else if (deathreason == "death_battle")
				TheString += "  " + death + ".1.1 = { \n   death = { death_reason = death_battle } \n  }";
			else if (deathreason == "death_rabble")
				TheString += "  " + death + ".1.1 = { \n   death = { death_reason = death_rabble } \n  }";
			else if (deathreason == "death_missing")
				TheString += "  " + death + ".1.1 = { \n   death = { death_reason = death_missing } \n  }";
			else if (deathreason == "death_inbred")
				TheString += "  " + death + ".1.1 = { \n   death = { death_reason = inbred } \n  }";
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
		return CurrentYear - birth;
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
