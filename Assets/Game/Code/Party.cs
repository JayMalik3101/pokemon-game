using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Party : MonoBehaviour
{

    public GameObject InterfaceButton;

    public List<Monster> Team;

    public bool AllDead;

    // Use this for initialization
    public void Init()
    {
        foreach (Monster monster in Team)
        {
            monster.CurrentHealth = monster.Health;
        }
    }

    public void PopulateParty()
    {
        GameObject list = transform.Find("Overview").gameObject;
        foreach (Transform obj in list.transform)
            Destroy(obj.gameObject);

        foreach (Monster monster in Team)
        {
            GameObject newMember = Instantiate(InterfaceButton, Vector3.zero, Quaternion.identity) as GameObject;
            newMember.transform.SetParent(list.transform, false);
            newMember.transform.Find("Name").GetComponent<Text>().text = monster.Name;
            newMember.transform.Find("Health").GetComponent<Text>().text = monster.CurrentHealth + "/" + monster.Health + "HP";
            int index = Team.IndexOf(monster);
            newMember.GetComponent<Button>().onClick.AddListener(() => MoveToFirst(index));
        }
    }

    public void RefreshBallInfo()
    {
        transform.Find("Text").GetComponent<Text>().text = GameManager.instance.StartingPokeballs + " pokéballs left!";
    }

    public void MoveToFirst(int index)
    {
        Monster member = Team[index];
        Team.RemoveAt(index);
        Team.Insert(0, member);
        PopulateParty();
    }

    public void Add(Monster monster)
    {
        if (Team.Count < 6)
            Team.Add(monster);
        GameManager.instance.DexUI.GetComponent<Pokedex>().Refresh();
    }
}
