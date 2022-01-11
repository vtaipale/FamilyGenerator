using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyGenerator : MonoBehaviour
{	
	public int currentYear = 500; //m41
	public int stopYear = 800;
	
	public List<int> funYears = new List<int>();
	public List<int> RewardYears = new List<int>();

	public int FunYearStart = 567;
	public int FunYearStop = 571;
	public int TotalCharNumber = 10;
	public GameObject FamilyMemberPrefab;
	
	public GameObject AliveMembers;
			
	public	int[] Dynasties  = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void ProcessYears(int howManyYears)
	{
		for (int index = 0; index < howManyYears; index++)
		{
			if (currentYear<=stopYear)
				this.ProcessYear();
		}
		
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
				FamilyPerson.death = currentYear + Random.Range(1,60);
			}
			else 
			{
				int FunDeathCheck = Random.Range(0,10);
				if ((FunDeathCheck > 5) && (FamilyPerson.traits.Contains("inbred")))
				{	
					FamilyPerson.Die("death_inbred");
				}
				else if (FunDeathCheck > 8)
				{	
					int DeathRandomiser = Random.Range (0, 10);

					FamilyPerson.death=currentYear;
					
					switch (DeathRandomiser) {
					case 0:
						FamilyPerson.murderer=GetRandomAdult();
						FamilyPerson.Die("death_murder");
						break;
					case 1:
						FamilyPerson.Die("death_murder_unknown");
						break;
					case 2:
						FamilyPerson.Die("death_battle");
						break;
					case 3:
						FamilyPerson.Die("death_battle");
						break;
					case 4:
						FamilyPerson.Die("death_rabble");
						break;
					case 5:
						FamilyPerson.Die("death_missing");
						break;
					case 6:
						FamilyPerson.Die("death_duel");
						break;
					case 7:
						FamilyPerson.murderer=GetRandomAdult();
						FamilyPerson.Die("death_duel");
						break;
					default:
						FamilyPerson.Die("death_murder_unknown");
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
					FamilyPerson.Die("death_missing"); //liian musta lammas
					Debug.Log("SHAME - " + FamilyPerson.GetFullName());
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
					Debug.Log("WOW - " + FamilyPerson.GetFullName());
					break;
				default:
					//nothing special
					break;
				}
				
			
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
			
			if (FamilyPerson.marriageyear > 0)
			{
				int FertilityCheck = Random.Range(1,100);

				if (FamilyPerson.traits.Contains("inbred"))
					FertilityCheck = (FertilityCheck*3);
				
				if ((Age <= 30) && (FertilityCheck < 17))
				{
					this.CreateNewMember(FamilyPerson);
				}		
				else if ((Age <= 35) && (FertilityCheck < 15))
				{
					this.CreateNewMember(FamilyPerson);
				}
				else if ((Age <= 40) && (FertilityCheck < 5))
				{
					this.CreateNewMember(FamilyPerson);
				}
				else if ((Age < 50) && (FertilityCheck < 2))
				{
					this.CreateNewMember(FamilyPerson);
				}
				else if (FertilityCheck <= 1)
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
		
		if (UseParentName < 3 && HappyParent.female == NewKid.female )
		{
			NewKid.charname = HappyParent.charname;
		}
		else if (NewKid.female == false) {// yes doublenames
			NewKid.charname = MaleFirstNames [(Mathf.RoundToInt (Random.value * (MaleFirstNames.GetLength (0) - 1)))];
			
			if (UseParentName > 8 && HappyParent.female == false )
				NewKid.charextranames = HappyParent.charname + " Jr.";
			else
				NewKid.charextranames = MaleFirstNames [(Mathf.RoundToInt (Random.value * (MaleFirstNames.GetLength (0) - 1)))];
				//NewKid.charextranames += " " +  FemaleFirstNames [(Mathf.RoundToInt (Random.value * (FemaleFirstNames.GetLength (0) - 1)))];
			
		} else {
			NewKid.charname = FemaleFirstNames [(Mathf.RoundToInt (Random.value * (FemaleFirstNames.GetLength (0) - 1)))];
			if (UseParentName > 8 && HappyParent.female == true )
				NewKid.charextranames = HappyParent.charname + " Jr.";
			else
				NewKid.charextranames = FemaleFirstNames [(Mathf.RoundToInt (Random.value * (FemaleFirstNames.GetLength (0) - 1)))];
				//NewKid.charextranames += " " + FemaleFirstNames [(Mathf.RoundToInt (Random.value * (FemaleFirstNames.GetLength (0) - 1)))];
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
			NewKid.Importance = HappyParent.Importance-1;
			NewKid.Prestige = NewKid.Importance + Mathf.RoundToInt(HappyParent.Prestige/10);
			
			if (HappyParent.spouse != null && HappyParent.spouse.living == true)
			{
				if (HappyParent.culture == "childOfDynasty" && (HappyParent.spouse.culture != "noble"))
				{
					NewKid.culture = "childOfDynasty";
					NewKid.FatePoints--;
					NewKid.addTrait("inbred");
					Debug.Log("Oh nou! Inbread:" + NewKid.GetFullName()); 
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
		
		NewMember.transform.SetParent(HappyParent.transform);
		
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
					
					int DeathRandomiser = Random.Range (0, 8);

					switch (DeathRandomiser) {

					case 0:
						Victim.Die("death_murder");
						Victim.murderer=GetRandomAdult(true);
						break;
					case 1:
						Victim.Die("death_murder_unknown");
						break;
					case 2:
						Victim.Die("death_murder");
						Victim.murderer=GetRandomAdult();
						break;
					case 3:
						Victim.Die("death_murder");
						Victim.murderer=GetRandomAdult(false);
						break;
					case 4:
						Victim.Die("death_missing");
						break;
					case 5:
						Victim.Die("death_duel");
						Victim.murderer=GetRandomAdult(false);
						break;
					case 6:
						Victim.Die("death_duel");
						break;
					default:
						Victim.Die("death_murder_unknown");
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
				return false;
			}
		}
		
		Debug.Log ("DYNASTIES OK!");
		return true;
		
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
		"falconer",
		"poet",
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
		"dynastic_kinslayer",
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
	
	string[] MaleFirstNames = new string[] {
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		
		"Smirnov",
		"Henrik",
		"James",

		"Bolton",
		"Arken",
		"Damien",
		"Piper",
		"Tybalt",
		"Torres",

		"Gavais",
		"Bort",
		"Jerry",

		"Bathul",
		"Ketil",
		"Erue",

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
		"Luis"

	};
	string[] FemaleFirstNames = new string[] {
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		
		"Jane",
		"Mary",
		"Rose",
		"Emily",
		"Vyce",
		"Felixia",
		"Alexandria",
		"Gweythe",
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
		"Sarah",
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
		"Jenessa",
		"Joan",
		"Judi",
		"Marie",
		"Mercy",
		"Misty",
		"Kiara",
		"Rexa"

	};
}
