using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyTreeMaker : MonoBehaviour
{
	
	public FamilyMember FamilyRoot;
	
	//public int sideChange = 0;
	
    // Start is called before the first frame update
    void Start()
    {
      this.setupkids();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void makeFamilyTree ()
	{
		//sideChange = 0;
		
		//this.setupkids();
		
		MoveFamilyMember(FamilyRoot, 0, 0);
		
	}
	
	public void setupkids ()
	{
		foreach (FamilyMember Person in FamilyRoot.GetComponentsInChildren<FamilyMember>())
		{
			//Person.getImportantParent().MyKids.Add(Person);
			
			
			if (Person.father != null)
			{
				Person.father.MyKids.Add(Person);
			}
			if (Person.mother != null)
			{
				Person.mother.MyKids.Add(Person);
			}
			
		}
		
	}
	
	private void MoveFamilyMember(FamilyMember CurrentPerson, int KidNumber, int sideChange)
	{
		//CurrentPerson.gameObject.transform.position.y = (CurrentPerson.getImportantParent().gameObject.transform.position.y-1);
		
		//CurrentPerson.gameObject.transform.position.x = (CurrentPerson.getImportantParent().gameObject.transform.position.x-KidNumber);
		
		CurrentPerson.transform.position = CurrentPerson.getImportantParent().transform.position + new Vector3(sideChange,-1,0);
		
		//CurrentPerson.transform.localPosition = new Vector3(0,-1, 0);
		//CurrentPerson.transform.localPosition = CurrentPerson.transform.localPosition + new Vector3(sideChange,0, 0);
		//CurrentPerson.transform.position = CurrentPerson.transform.position + new Vector3(KidNumber,0, 0);

		if (CurrentPerson.MyKids.Count > 0)
		{
		
			for (int index = 0; index < CurrentPerson.MyKids.Count; index++)
			{
				if (index > 0)
					sideChange++;
				
				MoveFamilyMember(CurrentPerson.MyKids[index],index,sideChange);
			}
		}

	}
	
}
