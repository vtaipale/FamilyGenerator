using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///Constructs a readable family tree
public class FamilyTreeMaker : MonoBehaviour
{
	
	public FamilyMember FamilyRoot;
	
	//public int sideChange = 0;
	
	public Mesh DeadMesh;
	
    // Start is called before the first frame update
    void Start()
    {
      //this.setupkids();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void makeFamilyTree ()
	{
		
		this.setupkids();
		
		MoveFamilyMember(FamilyRoot, 0);
		
		this.HandleDeadPersons(805);
		
	}
	
	public void setupkids ()
	{
		foreach (FamilyMember Person in FamilyRoot.GetComponentsInChildren<FamilyMember>())
		{
			Person.getImportantParent().MyKids.Add(Person);
			
			/*
			if (Person.father != null)
			{
				Person.father.MyKids.Add(Person);
			}
			if (Person.mother != null)
			{
				Person.mother.MyKids.Add(Person);
			}
			*/
		}
		
	}
	
	public void HandleDeadPersons(int CurrentYear)
	{
		foreach (FamilyMember Person in FamilyRoot.GetComponentsInChildren<FamilyMember>())
		{
			if (Person.death < CurrentYear)
		
			Person.transform.GetComponent<MeshFilter>().mesh =  DeadMesh;
	
		}
		
		
	}
	
	private int MoveFamilyMember(FamilyMember CurrentPerson, int KidNumber)
	{
		int howmuchMoved = 0;
		
		//CurrentPerson.gameObject.transform.position.y = (CurrentPerson.getImportantParent().gameObject.transform.position.y-1);
		
		//CurrentPerson.gameObject.transform.position.x = (CurrentPerson.getImportantParent().gameObject.transform.position.x-KidNumber);
		
		CurrentPerson.transform.position = CurrentPerson.getImportantParent().transform.position + new Vector3(KidNumber,-1,0);
		
		//CurrentPerson.transform.position = new Vector3(sideChange,-CurrentPerson.Generation,0);
		
		
		//CurrentPerson.transform.localPosition = new Vector3(0,-1, 0);
		//CurrentPerson.transform.localPosition = CurrentPerson.transform.localPosition + new Vector3(sideChange,0, 0);
		//CurrentPerson.transform.position = CurrentPerson.transform.position + new Vector3(KidNumber,0, 0);

		if (CurrentPerson.MyKids.Count > 0)
		{
		
			for (int index = 0; index < CurrentPerson.MyKids.Count; index++)
			{
				if (index > 0)
					howmuchMoved++;
				
				howmuchMoved += MoveFamilyMember(CurrentPerson.MyKids[index],howmuchMoved);
			}
		}
		
		return howmuchMoved;

	}
	
}
