using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FamilyGenerator : MonoBehaviour
{	
	public int currentYear = 500; //m41
	public int stopYear = 800;
	
	public List<int> funYears = new List<int>();
	public List<int> RewardYears = new List<int>();

	public int TotalCharNumber = 10;
	public GameObject FamilyMemberPrefab;
	
	public GameObject AliveMembers;
			
	public	int[] Dynasties  = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

	public bool InstantStart = false;
	public bool InstantReStart = false;

    // Start is called before the first frame update
    void Start()
    {
		if (InstantStart == true)
			this.ProcessYears(500);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void ProcessYears(int howManyYears)
	{
		float startTime = Time.time;
		
		for (int index = 0; index < howManyYears; index++)
		{
			if (currentYear<=stopYear)
				this.ProcessYear();
		}
		
		Debug.Log("Processed " +howManyYears+ "Years!  Duration:" + (Time.time-startTime));
		
		this.CheckFamilies();

	}
	public void ProcessYear()
	{
		
		int TotalCharNumberStart = TotalCharNumber;

		int AliveMembers = 0;
		int Deaths = 0;
		string DeadOnes = "";
		
		foreach (FamilyMember Person in FindObjectsOfType<FamilyMember>())
		{
			if (Person.processed == false && Person.living == true)
			{
				ProcessPerson(Person);
				
				if ((funYears.Contains(currentYear)) && (Person.historic == false))
				{
					funYear(Person);
				}
				if (Person.living == true)
					AliveMembers++;
				else
				{
					Deaths++;
					DeadOnes += Person.GetFullName() + " " + Person.deathreason + " \n" ;
				}
			}
		}
		
		if ((RewardYears.Contains(currentYear))) //little candy for survibors
		{
			foreach (FamilyMember Person in FindObjectsOfType<FamilyMember>())
			{	
				if (Person.living == true)
					{
						Person.addTrait("scarred");
						Person.FatePoints++;
					}
				}
			}
		
		foreach (ScriptedBirth Destiny in FindObjectsOfType<ScriptedBirth>())
		{
			if (Destiny.BirthYear == currentYear)
			{
				FamilyMember DestinyKid = CreateNewMember(Destiny.ProudParent, true);
				if (Destiny.DynastyInt > 0)
				{
					DestinyKid.dynasty = Destiny.DynastyInt;
					DestinyKid.dynastystring = Destiny.DynastyString;
				}
				DestinyKid.FatePoints = Destiny.BabyFate;
				
				DestinyKid.transform.position = Destiny.transform.position;
				Destroy(Destiny.gameObject,5f);
			}	
		}
		int NewKids = TotalCharNumber - TotalCharNumberStart;

		Debug.Log("YEAR " + currentYear + "m41 \n Alive: "+AliveMembers+" | Deaths: "+Deaths+" | New: "+NewKids + " \n" + DeadOnes);

		currentYear++;
	}
	
	public bool ProcessPerson(FamilyMember FamilyPerson)
	{
		int Age = FamilyPerson.getAge(currentYear);
		
		if (currentYear == FamilyPerson.death) //DeathCheck
		{
			if (FamilyPerson.historic == true)
			{
				FamilyPerson.Die();
						return false;
			}
			else if (FamilyPerson.FatePoints > 0)
			{
				FamilyPerson.FatePoints--;
				FamilyPerson.death = currentYear + Random.Range(1,60+FamilyPerson.Importance);
			}
			else 
			{
				int FunDeathCheck = Random.Range(0,10);
				if ((FunDeathCheck > 5) && (FamilyPerson.traits.Contains("inbred")))
				{	
					FamilyPerson.Die("inbred");
				}
				else if (FunDeathCheck > 8)
				{	
					int DeathRandomiser = Random.Range (0, 12);

					FamilyPerson.death=currentYear;
					
					switch (DeathRandomiser) {
					case 0:
						FamilyPerson.murderer=GetRandomAdult();
						FamilyPerson.Die("murder");
						FamilyPerson.murderer.kills.Add(FamilyPerson);
						FamilyPerson.murderer.addTrait("kinslayer");
						break;
					case 1:
						FamilyPerson.Die("murder_unknown");
						break;
					case 2:
						FamilyPerson.Die("battle");
						break;
					case 3:
						FamilyPerson.Die("battle");
						break;
					case 4:
						FamilyPerson.Die("rabble");
						break;
					case 5:
						FamilyPerson.Die("missing");
						break;
					case 6:
						FamilyPerson.Die("duel");
						break;
					case 7:
						FamilyPerson.murderer=GetRandomAdult();
						FamilyPerson.Die("duel");
						FamilyPerson.murderer.kills.Add(FamilyPerson);
						FamilyPerson.murderer.addTrait("kinslayer");
						break;
					case 8:
						FamilyPerson.Die("vanished");
						break;
					case 9:
						FamilyPerson.Die("duel");
						break;
					default:
						FamilyPerson.Die("murder_unknown");
						break;
					}
					
					return false;
				}				
				else 
				{
					int DeathCheck = Random.Range(1,100);
					
					if ((Age < 10) && (DeathCheck > 80))
					{
						FamilyPerson.Die();
						return false;
					}
					else if ((Age < 30) && (DeathCheck > 70))
					{
						FamilyPerson.Die();
						return false;
					}
					else if ((Age < 60) && (DeathCheck > 60))
					{
						FamilyPerson.Die();
						return false;
					}
					else if ((Age < 100) && (DeathCheck > 50))
					{
						FamilyPerson.Die();
						return false;
					}
					else if( DeathCheck > 40)
					{
						FamilyPerson.Die();
						return false;
					}
				}
				//if no death, randomise year for next death check. Importance gives more years
				FamilyPerson.death = currentYear + Random.Range(1,50+FamilyPerson.Importance);
			}
		}
		
		if (Age == 30 && (FamilyPerson.historic == false)) // NohevuusCheck
		{
			int NohevuusCheck = Mathf.RoundToInt(Random.Range(-5,5));
			
			FamilyPerson.Importance +=NohevuusCheck;
			FamilyPerson.Prestige +=NohevuusCheck;
			
			switch (NohevuusCheck) {

				case -4:
					FamilyPerson.death = currentYear;
					FamilyPerson.Die("missing"); //liian musta lammas
					//Debug.Log("SHAME - " + FamilyPerson.GetFullName());
					break;
				case -3:
					FamilyPerson.addTrait(ReallyBadTraits [(Mathf.RoundToInt (Random.value * (ReallyBadTraits.GetLength (0) - 1)))]);
					FamilyPerson.name = "XX-" +FamilyPerson.GetFullName();
					break;
				case -2:
					FamilyPerson.addTrait(BadTraits[(Mathf.RoundToInt (Random.value * (BadTraits.GetLength (0) - 1)))]);
					FamilyPerson.name = "X-" +FamilyPerson.GetFullName();
					break;
				case 3:
					FamilyPerson.name = "*-" +FamilyPerson.GetFullName();
					FamilyPerson.addTrait(GoodTraits [(Mathf.RoundToInt (Random.value * (GoodTraits.GetLength (0) - 1)))]);
					break;
				case 4:
					FamilyPerson.addTrait(ReallyGoodTraits [(Mathf.RoundToInt (Random.value * (ReallyGoodTraits.GetLength (0) - 1)))] );
					FamilyPerson.name = "**-" +FamilyPerson.GetFullName();
					FamilyPerson.FatePoints++;
					//Debug.Log("WOW - " + FamilyPerson.GetFullName());
					break;
				default:
					//nothing special
					break;
				}
				
			if (Mathf.RoundToInt(Random.Range(0,100)) > 96) //fun extra 
			{
				if (Mathf.RoundToInt(Random.Range(0,10)) > 5)
					FamilyPerson.addTrait("poet");
				else
					FamilyPerson.addTrait("falconer");
			}
			
			FamilyPerson.Motivation = Motivations [(Mathf.RoundToInt (Random.value * (Motivations.GetLength (0) - 1)))];

				
			
		}
		
		
		if ((FamilyPerson.historic == false) && ((Age >= 15) && (Age < 80) && (FamilyPerson.Importance > 8) && (FamilyPerson.dynastystring != "the Bastard")))
		{
			if (FamilyPerson.marriageyear == 0)
			{
				int MarriageRoll = Random.Range(1,100);
				
				if ((Age < 20) && (MarriageRoll < FamilyPerson.Importance))
				{
					MarriageCheck(FamilyPerson);
				}		
				else if ((Age >= 20) && (Age < 45) && (MarriageRoll < FamilyPerson.Importance*2))
				{
					MarriageCheck(FamilyPerson);
				}			
				else if (MarriageRoll < FamilyPerson.Prestige)
				{
					this.CreateNewMember(FamilyPerson);
				}
				
			}
			
			if ((FamilyPerson.marriageyear > 0) && (FamilyPerson.Fertility > 0))
			{
				int FertilityCheck = Random.Range(1,100);

				if (FamilyPerson.traits.Contains("inbred"))
					FertilityCheck = (FertilityCheck*3);
				
				if ((Age <= 30) && (FertilityCheck < 17*FamilyPerson.Fertility))
				{
					this.CreateNewMember(FamilyPerson);
				}		
				else if ((Age <= 35) && (FertilityCheck < 15*FamilyPerson.Fertility))
				{
					this.CreateNewMember(FamilyPerson);
				}
				else if ((Age <= 40) && (FertilityCheck < 5*FamilyPerson.Fertility))
				{
					this.CreateNewMember(FamilyPerson);
				}
				else if ((Age < 50) && (FertilityCheck < 2*FamilyPerson.Fertility))
				{
					this.CreateNewMember(FamilyPerson);
				}
				else if (FertilityCheck <= 1*FamilyPerson.Fertility)
				{
					this.CreateNewMember(FamilyPerson);
				}
			}
			
			/* old style
			if ((Age < 20) && (FertilityCheck < 3))
			{
				this.CreateNewMember(FamilyPerson);
			}		
			else if ((Age < 30) && (FertilityCheck < 5))
			{
				this.CreateNewMember(FamilyPerson);
			}
			else if ((Age < 45) && (FertilityCheck < 10))
			{
				this.CreateNewMember(FamilyPerson);
			}
			else if ((Age < 60) && (FertilityCheck < 5))
			{
				this.CreateNewMember(FamilyPerson);
			}
			else if (FertilityCheck <= 2)
			{
				this.CreateNewMember(FamilyPerson);
			} */
		}
		
		return true;
		
	}
	
	public FamilyMember CreateNewMember(FamilyMember HappyParent)
	{
		if (HappyParent.spouse != null && HappyParent.spouse.living == true)
		{
			if (HappyParent.spouse.dynastystring == "the Bastard")
				return this.CreateNewMember(HappyParent,false);
			
			else if ((HappyParent.spouse.Importance > HappyParent.Importance))
			{
				return this.CreateNewMember(HappyParent.spouse,true);
			}
		}
		
		return this.CreateNewMember(HappyParent, false);
	}
	public FamilyMember CreateNewMember(FamilyMember HappyParent, bool noBastards)
	{
		//Debug.Log("NEW KID!");
		
		GameObject NewMember = (GameObject)Instantiate (FamilyMemberPrefab, (HappyParent.transform.position + new Vector3(0,-1,0)), HappyParent.transform.rotation);
		//GameObject NewMember = (GameObject)Instantiate (FamilyMemberPrefab, (HappyParent.transform.position), HappyParent.transform.rotation);
	
		NewMember.transform.SetParent(HappyParent.transform);
		
		FamilyMember NewKid = NewMember.GetComponent <FamilyMember>();

		TotalCharNumber++;
		NewKid.charnumber = TotalCharNumber;
		
		int SexRandomiser = Random.Range (0, 2);

		switch (SexRandomiser) {

			case 0:
				NewKid.female = false;
				break;
			case 1:
				NewKid.female = true;
				break;
			default:
				NewKid.female = true;
				break;
		}
		int UseParentName = Random.Range(0,10);
		
		if (UseParentName < 2 && HappyParent.female == NewKid.female ) //name after parent
		{
			NewKid.charname = HappyParent.charname;
		}
		else if (UseParentName > 8 && HappyParent.getImportantParent() == NewKid.female ) //name after grandparent
		{
				NewKid.charname = HappyParent.getImportantParent().charname;
		}
		
		else if (NewKid.female == false) {// yes doublenames
			NewKid.charname = MaleFirstNames [(Mathf.RoundToInt (Random.value * (MaleFirstNames.GetLength (0) - 1)))];
			
			/*
			if (UseParentName < 3 && HappyParent.getImportantParent().female == false )
				NewKid.charextranames = HappyParent.getImportantParent().charname;
			else if (UseParentName > 8 && HappyParent.female == false )
				NewKid.charextranames = HappyParent.charname + " Jr.";
			else
				NewKid.charextranames = MaleFirstNames [(Mathf.RoundToInt (Random.value * (MaleFirstNames.GetLength (0) - 1)))];*/
				//NewKid.charextranames += " " +  FemaleFirstNames [(Mathf.RoundToInt (Random.value * (FemaleFirstNames.GetLength (0) - 1)))];
			
		} else {
			NewKid.charname = FemaleFirstNames [(Mathf.RoundToInt (Random.value * (FemaleFirstNames.GetLength (0) - 1)))];
			/*
			if (UseParentName < 3 && HappyParent.getImportantParent().female == true )
				NewKid.charextranames  = HappyParent.getImportantParent().charname;
			else if (UseParentName > 8 && HappyParent.female == true )
				NewKid.charextranames = HappyParent.charname + " Jr.";
			else
				NewKid.charextranames = FemaleFirstNames [(Mathf.RoundToInt (Random.value * (FemaleFirstNames.GetLength (0) - 1)))];
				//NewKid.charextranames += " " + FemaleFirstNames [(Mathf.RoundToInt (Random.value * (FemaleFirstNames.GetLength (0) - 1)))];*/
		}
		
		if (NewKid.historic == false)
			{
				int NumberAncestorsHaveSameName = NewKid.CheckAncestorsForName();

				if (NumberAncestorsHaveSameName > 1) //urg ugly
				{
					if (NumberAncestorsHaveSameName == 2)
						NewKid.charextranames += "II ";
					else if (NumberAncestorsHaveSameName == 3)
						NewKid.charextranames += "III ";
					else if (NumberAncestorsHaveSameName == 4)
						NewKid.charextranames += "IV ";
					else if (NumberAncestorsHaveSameName == 5)
						NewKid.charextranames += "V ";
					else if (NumberAncestorsHaveSameName == 6)
						NewKid.charextranames += "VI ";
					else if (NumberAncestorsHaveSameName == 7)
						NewKid.charextranames += "VII ";
					else if (NumberAncestorsHaveSameName == 8)
						NewKid.charextranames += "VIII ";
					else if (NumberAncestorsHaveSameName == 9)
						NewKid.charextranames += "IX ";
					else if (NumberAncestorsHaveSameName == 10)
						NewKid.charextranames += "X ";
					else
						NewKid.charextranames += "X? ";
				}
			}
		
		 if (NewKid.female == false)
		{
			NewKid.charextranames += MaleFirstNames [(Mathf.RoundToInt (Random.value * (MaleFirstNames.GetLength (0) - 1)))];
		}
		else
		{
			NewKid.charextranames += FemaleFirstNames [(Mathf.RoundToInt (Random.value * (FemaleFirstNames.GetLength (0) - 1)))];
		}
		
		if (HappyParent.female == false)
			NewKid.father = HappyParent;
		else
			NewKid.mother = HappyParent;
		
		NewKid.culture = HappyParent.culture;
			
		int BastardCheck = Random.Range(0,10);
			
		if (BastardCheck > 2 | noBastards == true)
		{
			NewKid.dynasty = HappyParent.dynasty;
			NewKid.dynastystring = HappyParent.dynastystring;
			//NewKid.Fertility = HappyParent.Fertility*Random.Range(0.95f,1.05f);
			NewKid.Importance = HappyParent.Importance-1;
			NewKid.Prestige = NewKid.Importance + Mathf.RoundToInt(HappyParent.Prestige/10);
			
			if (HappyParent.spouse != null && HappyParent.spouse.living == true)
			{
				if (HappyParent.culture == "childOfDynasty" && (HappyParent.spouse.culture != "noble"))
				{
					NewKid.culture = "childOfDynasty";
					NewKid.FatePoints--;
					NewKid.addTrait("inbred");
					//Debug.Log("Oh nou! Inbread:" + NewKid.GetFullName()); 
				}
				else if (HappyParent.culture == "noble" | (HappyParent.spouse.culture == "noble"))
					NewKid.culture = "childOfDynasty";
				else if (HappyParent.dynasty != HappyParent.spouse.dynasty)
				{
					NewKid.culture = "noble";
				}
				
				NewKid.Prestige += Mathf.RoundToInt(HappyParent.spouse.Prestige/10);
		
				if (HappyParent.spouse.female == false)
					NewKid.father = HappyParent.spouse;
				else
					NewKid.mother = HappyParent.spouse;
			}
			
		}
		else // a bastard!
		{
			NewKid.dynasty = HappyParent.dynasty;	
			NewKid.dynastystring = "the Bastard";
			//NewKid.Fertility = HappyParent.Fertility*Random.Range(0.9f,1.1f);
			NewKid.Importance = 8;
			NewKid.Prestige =  HappyParent.Prestige;
			NewKid.addTrait("bastard");
		}
		
		if (NewKid.Importance > 12) //chance fer important to be more fatey
		{
			NewKid.FatePoints += Mathf.Max (0, Mathf.RoundToInt(Random.Range(-3,2)));
		}
		
		NewKid.Generation = HappyParent.Generation + 1;
		
		
		
		//NewKid.addTrait("proud");
		NewKid.addTrait(AverageTraits [(Mathf.RoundToInt (Random.value * (AverageTraits.GetLength (0) - 1)))]);
		
		NewKid.birth = currentYear;
		
		NewKid.death = currentYear + Random.Range(1,70+NewKid.Importance);
		
		if (NewKid.culture == "childOfDynasty")
			NewKid.death = Mathf.Max (NewKid.death-10, currentYear+1);
		
		//HappyParent.Descendants
		
		
		//Debug.Log("NEW KID!");
		
		return NewKid;
	}
	
	public void funYear(FamilyMember Victim)
	{
			if (Victim.FatePoints > 0)
			{
				Victim.FatePoints--;
				Victim.addTrait("scarred_mid");
			}
			else 
			{
				int FunDeathCheck = Random.Range(0,10);
				
				if (FunDeathCheck > 7)
				{	
			
					Victim.death=currentYear;
					
					int DeathRandomiser = Random.Range (0, 15);

					switch (DeathRandomiser) {

					case 0:
						Victim.Die("murder");
						Victim.murderer=GetRandomAdult(true);
						Victim.murderer.kills.Add(Victim);
						Victim.murderer.addTrait("kinslayer");
						break;
					case 1:
						Victim.Die("murder_unknown");
						break;
					case 2:
						Victim.Die("murder");
						Victim.murderer=GetRandomAdult();
						Victim.murderer.kills.Add(Victim);
						Victim.murderer.addTrait("kinslayer");
						break;
					case 3:
						Victim.Die("murder");
						Victim.murderer=GetRandomAdult(false);
						Victim.murderer.kills.Add(Victim);
						Victim.murderer.addTrait("kinslayer");
						break;		
					case 4:
						Victim.Die("battle");
						break;
					case 5:
						Victim.Die("missing");
						break;
					case 6:
						Victim.Die("duel");
						Victim.murderer=GetRandomAdult(false);
						Victim.murderer.kills.Add(Victim);
						Victim.murderer.addTrait("kinslayer");
						break;
					case 7:
						Victim.Die("duel");
						break;
					case 8:
						Victim.Die("dungeon");
						Victim.murderer=GetRandomAdult(false);
						Victim.murderer.kills.Add(Victim);
						Victim.murderer.addTrait("kinslayer");
						break;
					case 9:
						Victim.Die("drank_poison");
						break;
					case 10:
						Victim.Die("vanished");
						break;	
					case 11:
						Victim.Die("execution");
						Victim.murderer=GetRandomAdult(true);
						Victim.murderer.kills.Add(Victim);
						Victim.murderer.addTrait("kinslayer");
						break;
					default:
						Victim.Die("murder_unknown");
						break;
					}
					
				}
			}
	}
	
	public bool CheckFamilies()
	{
		Dynasties  = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		
		int AliveMembers = 0;
		int DynastyChildren = 0;
		int Nobleborns = 0;
		int Inbreds = 0;
		
		foreach (FamilyMember Person in FindObjectsOfType<FamilyMember>())
		{	
			if (Person.living == true)
				{
					AliveMembers++;
					Dynasties[Person.dynasty]++;
					
					if (Person.culture == "childOfDynasty"){
						DynastyChildren++;
						if (Person.traits.Contains("inbred"))
							Inbreds++;
					}
					else if (Person.culture == "noble")
						Nobleborns++;
				}
		}
		
		
		Debug.Log ("DYNASTYCHECK; \n Alive: "+AliveMembers+" | Nobleborns: "+Nobleborns+" | DynastyChilds: "+DynastyChildren + " | Inbreds:" + Inbreds);
		
		
		for (int index = 1; index < Dynasties.Length-1; index++)
		{
			if (Dynasties[index] == 0)
			{
				Debug.LogWarning ("ERROR no members in dynasty " + index + "!");
				if (InstantReStart == true)
				{
					this.LoadNewScene(1);
				}
				
				return false;

			}
		}
		
		Debug.Log ("DYNASTIES OK!");
		return true;
		
	}
	
	
	public void LoadNewScene(int sceneToLoad)
	{
		SceneManager.LoadScene(sceneToLoad);
	}
	
	public bool MarriageCheck(FamilyMember WantsMarried)
	{
		if (WantsMarried.marriageyear != 0)
			return false; //wtf trying to marry a married person
		
		int MarriageInFamily = Random.Range(1,100);
				
		if (MarriageInFamily > (WantsMarried.Importance+WantsMarried.Prestige)) //more important members marry inside family
		{
			WantsMarried.marriageyear = currentYear;
			Debug.Log("Marriage! : " + WantsMarried.GetFullName() + " ( age"+WantsMarried.getAge(currentYear)+") married outside the Dynasty" );

			return true;
		}	
		
		List<FamilyMember> PotentialPartners = new List<FamilyMember>();
		
		foreach (FamilyMember Person in FindObjectsOfType<FamilyMember>())
		{	
			if (Person.living == true && Person.getAge(currentYear) > 15 && Person.marriageyear == 0 && Person.historic == false && Person.female == !WantsMarried.female)	
			{
				PotentialPartners.Add(Person);
			}
		}
		
		FamilyMember[] PotentialPartnersArray = PotentialPartners.ToArray();

		if (PotentialPartnersArray.Length == 0)
			return false; //no marriage :(

		FamilyMember TheSpouse = PotentialPartnersArray [(Mathf.RoundToInt (Random.value * (PotentialPartnersArray.GetLength (0) - 1)))];
		
		// They Do!
		WantsMarried.spouse = TheSpouse;
		TheSpouse.spouse = WantsMarried;
		
		WantsMarried.marriageyear = currentYear;
		TheSpouse.marriageyear = currentYear;

		Debug.Log("Marriage! : " + WantsMarried.GetFullName() + " ( age"+WantsMarried.getAge(currentYear)+") married " + TheSpouse.GetFullName() +" ( age"+TheSpouse.getAge(currentYear)+")!" );

		return true;
		
	}
	public List<FamilyMember> GetAllLivingAdults()
	{	
		return this.GetAllLivingAdults(false); 
	}
	
	public List<FamilyMember> GetAllLivingAdults(bool historicity)//marriages = no histori, murders, yes history
	{
		List<FamilyMember> AliveMembers = new List<FamilyMember>();
		
		foreach (FamilyMember Person in FindObjectsOfType<FamilyMember>())
		{	
			if (Person.living == true && Person.getAge(currentYear) > 15 && Person.historic == historicity )	
			{
				AliveMembers.Add(Person);
			}
		}
		return AliveMembers;
	}
	
	public List<FamilyMember> GetAllLivingAdults(int QuestionYear)
	{
		List<FamilyMember> AliveMembers = new List<FamilyMember>();
		
		foreach (FamilyMember Person in FindObjectsOfType<FamilyMember>())
		{	
			if (Person.birth <= QuestionYear && Person.death > QuestionYear )	
			{
				AliveMembers.Add(Person);
			}
		}
		return AliveMembers;
	}
	
	public List<FamilyMember> GetAllAdultsFiltered(string QuestionTrait, bool HasYesNo)
	{
		List<FamilyMember> MembersReturn = new List<FamilyMember>();
		
		foreach (FamilyMember Person in FindObjectsOfType<FamilyMember>())
		{	
		
			if (HasYesNo == true && Person.traits.Contains(QuestionTrait) )	
			{
				MembersReturn.Add(Person);
			}
		}
		return MembersReturn;
	}
	
	public List<FamilyMember> GetAllLivingAdultsFiltered(int QuestionYear, string QuestionTrait, bool HasYesNo)
	{
		List<FamilyMember> MembersReturn = new List<FamilyMember>();
		
		foreach (FamilyMember Person in this.GetAllLivingAdults(QuestionYear))
		{	
		
			if (HasYesNo == Person.traits.Contains(QuestionTrait) )	
			{
				MembersReturn.Add(Person);
			}
		}
		return MembersReturn;
	}	
	public List<FamilyMember> GetAllLivingAdultsFilteredByCulture(int QuestionYear, string QuestionTrait, bool HasYesNo)
	{
		List<FamilyMember> MembersReturn = new List<FamilyMember>();
		
		foreach (FamilyMember Person in this.GetAllLivingAdults(QuestionYear))
		{	
		
			if (HasYesNo == Person.culture.Equals(QuestionTrait) )	
			{
				MembersReturn.Add(Person);
			}
		}
		return MembersReturn;
	}
	
	public FamilyMember GetRandomAdult(bool historicity)
	{
		//List<FamilyMember> AliveMembers = GetAllLivingAdults(historicity);
		FamilyMember[] AliveMembers = GetAllLivingAdults(historicity).ToArray();

		return 	AliveMembers [(Mathf.RoundToInt (Random.value * (AliveMembers.GetLength (0) - 1)))];
		
	}
	
	public FamilyMember GetRandomAdult()
	{

		//List<FamilyMember> AliveMembers = GetAllLivingAdults(historicity);
		FamilyMember[] AliveMembers = GetAllLivingAdults(true).ToArray();

		if 		(Random.Range(0f,1f) > 0.5f)
			AliveMembers = GetAllLivingAdults(false).ToArray();

		return 	AliveMembers [(Mathf.RoundToInt (Random.value * (AliveMembers.GetLength (0) - 1)))];
		
	}

	string[] ReallyGoodTraits = new string[] {
		"genius",
		"fair",
		"physician",
		"administrator",
		"gamer",
		"strategist",
		"schemer",
		"mystic",
		"midas_touched",
		"elusive_shadow",
		"brilliant_strategist"

	};
	string[] GoodTraits = new string[] {
		"quick",
		"strong",
		"shrewd",
		"robust",
		"duelist",
		"hunter",
		"scholar",
		"socializer",
		"adventurer",
		"gladiator",	
		"pirate",	
		"paranoid",	
		"ambitious",	
		"content",	
		"zealous",
		"deceitful",
		"gregarious",
		"fortune_builder",
		"charismatic_negotiator",
		"tough_soldier",
		"skilled_tactician"

	};	
	string[] AverageTraits = new string[] {
		"dwarf",
		"giant",
		"lefthanded",
		"sturdy",
		"gardener",
		"temperate",
		"diligent",
		"patient",
		"proud",
		"lustful",
		"wroth",
		"brave",
		"cruel",
		"stressed",
		"stutter",
		"child_of_consort",
		"one_eyed",
		"groomed",
		"uncouth",
		"homosexual",
		"cancer"

	};
	string[] BadTraits = new string[] {
		"lunatic",
		"drunkard",
		"ugly",
		"ugly",
		"hunchback",
		"feeble",
		"dull",
		"scarred_high",
		"ill",
		"impaler",
		"craven",
		"arbitrary",
		"cynical",
		"pilgrim",
		"shy",
		"paranoid",	
		"mangled",
		"disfigured",
		"indulgent_wastrel",
		"is_fat"

	};
	string[] ReallyBadTraits = new string[] {
		"imbecile",
		"possessed",
		"hedonist",
		"theologian",
		"trusting",
		"celibate",
		"mastermind_theologian",
		"misguided_warrior",
		"amateurish_plotter",	
		"excommunicated",	
		"cannibal_trait",	
		"gladiator",
		"horse",
		"blinded",
		"leper"
	};
	string[] Motivations = new string[] {
		"Endurance",
		"Endurance",
		"Fortune",
		"Fortune",
		"Fortune",
		"Fortune",
		"Fortune",
		"Vengeance",
		"Vengeance",
		"Renown",
		"Renown",
		"Renown",
		"Pride",
		"Pride",
		"Pride",
		"Pride",
		"Prestige",
		"Prestige",
		"Devotion-Creed",
		"Devotion-Duty",
		"Devotion-Loyalty",
		"Knowledge-Is Life",
		"Knowledge-Know Thy Foe",
		"Knowledge-Is Power",
		"Fear-Enemy",
		"Fear-OwnSins",
		"Fear-Tormented",
		"Exhiliration-New Horizons",
		"Exhiliration-Thrill of War",
		"Exhiliration-Decadent"
	};
	string[] MaleFirstNames = new string[] {
       
       "Sean",       
	   "Sean",
	   "Sean",
       "Sean",
       "Laurelius",
       "Laurelius",
       "Laurelius",
       "Laurelius",
       "Laurelius",
       "Laurelius",
       "Laurelius",
       "Laurelius",
       "Laurelius",
       "Laurelius",
       "Rowland",
       "Rowland",
       "Migsett",
       "Robertus",
       "Robertus",
       "Jacob",

       "Tomazo",
       "Ambrosius",
       "Timoteus",
       "Matheus",

       "Victrix",
       "Macharius",
       "Casmir",
       "Lucien",
       "Roland",
       "Zadok",
       "Luc",
       "Jared",

       "Winston",
       "Lillard",
       "Ricimer",
       "Limeteti",
       "Rubens",
       "Sousa",

        "Smirnov",
        "Henrik",
        "James",

        "Bolton",
        "Arken",
        "Ervin",
        "Damien",
        "Piper",
        "Tybalt",
        "Torres",

        "Gavais",
        "Bort",
        "Jerry",

        "Frogor",
        "Karhu",
        "Wolfie",
        "Reba",
        "Kala",
        "Joh",
        "Mika",
        "Ernicos",
        "Kullervo",
        "Tumpelo",
        "Pax",
        "Miekka",

        "George",
        "Walt",
        "Tom",
        "Julius",
        "Aurelian",
        "Ostar",
        "Aum",
        "Darfee",
        "Leodos",
        "Charles",
        "Tikar",
        "Ronald",
        "Reuel",
        "Falcone",
        "Oscar",
        "Alexander",
        "Pavel",
        "Obriel",
        "Flavien",
        "Edmund",
        "Florian",

        "Isidu",
        "Jason",
        "Leroy",
        "Leon",
        "Martin",
        "Noel",
        "Ronald",
        "Thomas",
        "Victor",
        "Valerian",

        "Mikhail",
        "Clemency",
        "Clive",
        "David",
        "Eric",
        "Ragnar",
        "Gabriel",

        "Harrison",
        "Arnold",
        "Posius",
        "Stephen",
        "Henry",
        "Karl",
        "Luis",
//Roman
        "Pupius",
        "Hostus",
        "Titus",
        "Secundus",
        "Tertius",
        "Quintus",
        "Appius",
        "Domitius",
        "Aburius",
        "Hositius",
        "Gallus",
        "Decius",
        "Aemilius",
        "Tuccius",
        "Lar",
        "Geganius",
        "Numerius",
        "Marnius",
        "Placus",
        "Drusus",
        "Urgulanius",
        "Umbrenius",
        "Aburius",
        "Servius",
        "Marcus",
        "Maximus",
        "Statius",
        "Aulus",
        "Ulpius",
//Shakespearean
        "Camillo",
        "Fortinbras",
        "Aeneas",
        "Bartholomew",
        "Lear",
        "Simonides",
        "Philip",
        "Emmanuel",
        "Lysimachus",
        "Thomas",
        "Berowne",
        "Cleon",
        "Antenor",
        "Lennox",
        "Edmund",
        "Aaron",
        "Leontes",
        "Nathaniel",
        "Alonze",
        "Hero",
        "Antiochus",
        "Chatillon",
        "Lucio",
        "Dennis",
        "Publius",
        "Iago",
        "Philemon",

//Scottish
        "Sean",
        "Seth",
        "Corey",
        "Keegan",
        "Roebin",
        "Samuel",
        "Steven",
        "Maxwell",
        "Minwell",
        "Brandon",
        "Elias",
        "Corey",
        "Josh",
        "Harley",
        "Matthew",
        "Callan",
//Victorean
        "Orion",
        "Ernst",
        "Wyatt",
        "Hans",
        "Mark",
        "Lem",
        "Ronald",
        "Darrell",
        "Bessie",
        "Gene",
        "Jay",
        "Wilbur",
        "Hobson",
        "Arther",
        "Unknown",
        "Ellis",
        "Bailey",
        "Alpha",
        "Sid",
        "Ellis",
        "Antone",
        "Avery",
//AngloSax
        "Hunbald",
        "Scenwulf",
        "Cynwulf",
        "Tidfrith",
        "Burgweard",
        "Theobald",
        "Eadulf",
        "Hubert",
        "Delwyn",
        "Ewias",
        "Wilmaer",
        "Winfirth",
        "Medwin",
        "Goodwin",
        "Farma",
        "Osulf",
        "Eomer",
        "Colman",
        "Wilgils",
        "Baerwald",
        "Coleman",
        "Helmheard",
//Italian

        "Aureliano",
        "Orazio",
        "Aidano",
        "Giona",
        "Candido",
        "Brancaleone",
        "Geronzio",
        "Accursio",
        "Maurizio",
        "Bonaldo",
        "Sarbello",
        "Vitale",
        "Sante",
        "Umile",
    };
    
string[] FemaleFirstNames = new string[] {

        "Seuna",
        "Seuna",
        "Seuna",
        "Seuna",
        "Seuna",
        "Seuna",

        "Mariella",
        "Mariella",
        "Mariella",
        "Lettice",
        "Isadora",
        "Isadora",
        "Isadora",
        "Alenzea",
        "Lunastar",
        "Wendel",
        "Wendel",
        "Wendel",
        "Wendel",
        "Avis",
        "Avis",
        "Ellyn",
        "Julie",
        "Alexandra",
        "Alex",
        "Yara",
        "Rachel",
        "Zoe",
        "Theophina",

        "Jane",
        "Mary",
        "Rose",
        "Emily",
        "Vyce",
        "Felixia",
        "Alexandria",
        "Lily",
        "Catherine",
        "Oceania",
        "Laura",
        "Balia",
        "Nelma",
        "Ice",
        "Saurela",
        "Regina",
        "Nia",
        "Bella",
        "Vindi",
        "Peace",
        "Nancy",
        "Eliza",
        "Maura",
        "Ilona",

        "Emma",
        "Cora",
        "Victoria",
        "Eleanore",
        "Iris",
        "Isabel",
        "Natalie",
        "Maia",
        "Mirabelle",
        "Odette",
        "Penelope",
        "Seraphin",
        "Prudentia",
        "Valentia",
        "Zoe",
        "Unity",
        "Abigail",
        "Aleah",
        "Alise",
        "Angela",
        "Berenice",
        "Calista",
        "Carol",
        "Cheryl",
        "Danica",
        "Deanna",
        "Dora",
        "Evelyn",
        "Gladys",
        "Hailey",
        "Ida",
        "Isidora",
        "Isperia",
        "Iratha",
        "Iosefka",
        "Jenessa",
        "Joan",
        "Judi",
        "Marie",
        "Mercy",
        "Misty",
        "Kiara",
        "Rexa",
//Gothic
        "Elja",
        "Emelia",
        "Lorelei",
        "Gudeliva",
        "Amala",
        "Fredegonda",
        "Avagisa",
        "Buffy",
        "Seda",
        "Almawara",
        "Malasintha",
        "Helchen",
        "Avina",
        "Valdamerca",
        "Monia",
        "Rasha",
        "Lucienne",
        "Emalia",
        "Rasha",
        "Hermangild",
        "Avagisa",
        "Richildis",
        "Melle",
        "Kaethe",
        "Avina",
        "Teja",
        "Heidrun",
        "Mira",
        "Arika",
//Roman
        "Bantia",
        "Attia",
        "Clodia",
        "Desticia",
        "Visellia",
        "Sertoria",
        "Maelia",
        "Antistia",
        "Floria",
        "Sestia",
        "Maximilia",
        "Sextitia",
        "Antonia",
        "Insteia",
        "Cassia",
        "Sornatia",
        "Lucienne",
        "Minucia",
        "Ceionia",
        "Longinia",
        "Gabinia",
        "Dexia",
        "Vibia",
        "Piscia",
        "Ulpia",
        "Septimia",
        "Allectia",
        "Petrasia",
        "Helvetia",
//Shakespear
        "Cordelia",
        "Rosaline",
        "Timandra",
        "Luciana",
        "Iris",
        "Portia",
        "Lucetta",
        "Isidore",
        "Juno",
        "Margery",
        "Lavinia",
        "Alexas",
        "Mopsa",
        "Marina",
        "Iras",
        "Desdemona",
        "Valentine",
        "Luce",
        "Bianca",
        "Ursula",
        "Iras",
        "Constance",
        "Pucelle",
        "Page",
//Scott
        "Annie",
        "Alexandra",
        "Ellis",
        "Darcy",
        "Alicia",
        "Nora",
        "Lexie",
        "Jessica",
        "Carly",
        "Payton",
        "Ayesha",
        "Aimee",
        "Clara",
        "Caoimhe",
        "Abigail",
        "Amber",
//Victorian
        "Gretchen",
        "Ressie",
        "Adline",
        "Patricia",
        "Shirley",
        "Maude",
        "Joy",
        "Georgina",
        "Eulah",
        "Carolina",
        "Elma",
        "Adelina",
        "Honora",
        "Rosina",
        "Estelle",
        "Minta",
        "Huldah",
        "Odelia",
//Angosax
        "Hugeburc",
        "Brihtiue",
        "Wulfhild",
        "Leofrun",
        "Aedwen",
        "Cyneberg",
        "Golderon",
        "Emma",
        "Osburga",
        "Saxleue",
        "Adellufu",
        "Somerild",
        "Geatfleda",
        "Lefsuet",
        "Hildeguth",
        "Saegifu",
        "Eadguth",
        "Somerhild",
        "Edusa",
        "Wlfrun",
//Italic

        "Filippa",
        "Cirilla",
        "Ortesia",
        "Cunegonda",
        "Venera",
        "Savina",
        "Batilda",
        "Simonetta",
        "Palmira",
        "Estela",
        "Dorotea",
        "Diodata",
        "Allegra",
        "Cordelia",
        "Delizia",
        "Miriam",
        "Floridia",
        "Ines",
        "Livia",
        "Aurora",
        "Vissia"

    };


}
