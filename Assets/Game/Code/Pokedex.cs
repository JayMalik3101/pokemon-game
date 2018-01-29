using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Pokedex : MonoBehaviour
{
    public GameObject InterfaceButton;
    public Sprite NoInfoSprite;

    [HideInInspector]
    public List<GameObject> PokemonList;

    [HideInInspector]
    public List<string> SeenList;

    [HideInInspector]
    public bool AllCaught;

    // Use this for initialization
    public void Init()
    {
        PokemonList = new List<GameObject>();
        PokemonList.AddRange(Resources.LoadAll<GameObject>("Monsters"));

        SeenList = new List<string>();
    }

    public void See(string name)
    {
        if (!SeenList.Contains(name))
            SeenList.Add(name);
    }

    public void Refresh()
    {
        GameObject list = transform.Find("Scroll View/Viewport/Content").gameObject;
        foreach (Transform obj in list.transform)
            Destroy(obj.gameObject);

        int index = 1;
        int caught = 0;

        foreach (GameObject monster in PokemonList)
        {
            GameObject newMember = Instantiate(InterfaceButton, Vector3.zero, Quaternion.identity) as GameObject;
            newMember.transform.SetParent(list.transform, false);
            newMember.transform.Find("Number").GetComponent<Text>().text = index.ToString("D3");

            if (GameManager.instance.Team.Team.Find(m => m.Name.Equals(monster.GetComponent<Monster>().Name)))
            {
                if (!SeenList.Contains(monster.GetComponent<Monster>().Name))
                    SeenList.Add(monster.GetComponent<Monster>().Name);
                newMember.transform.Find("Name").GetComponent<Text>().text = monster.GetComponent<Monster>().Name;
                newMember.transform.Find("Caught").gameObject.SetActive(true);
                caught++;
            }
            else
            {
                if (SeenList.Contains(monster.GetComponent<Monster>().Name))
                    newMember.transform.Find("Name").GetComponent<Text>().text = monster.GetComponent<Monster>().Name;
                else
                    newMember.transform.Find("Name").GetComponent<Text>().text = "??????";
                newMember.transform.Find("Caught").gameObject.SetActive(false);

            }
            newMember.name = "PokedexItem";
            newMember.GetComponent<Button>().onClick.AddListener(() => Show(monster.GetComponent<Monster>().Name));
            AddEventTriggerListener(newMember.GetComponent<EventTrigger>(), EventTriggerType.PointerEnter, OnHover);
            AddEventTriggerListener(newMember.GetComponent<EventTrigger>(), EventTriggerType.PointerExit, OnLeave);

            index++;
        }

        if (caught == PokemonList.Count)
            AllCaught = true;
    }

    public void Show(string name)
    {

    }

    public void OnHover(BaseEventData eventData)
    {
        PointerEventData pointerEventData = (PointerEventData)eventData;
        string name = pointerEventData.pointerEnter.transform.Find("Name").GetComponent<Text>().text;
        transform.Find("Data/Name").GetComponent<Text>().text = name;

        GameObject monster = PokemonList.Find(m => m.GetComponent<Monster>().Name.Equals(name));
        if (monster == null)
        {
            transform.Find("Data/Pokemon").GetComponent<Image>().sprite = NoInfoSprite;
            transform.Find("Data/Panel/Text").GetComponent<Text>().text = "No info";
        }
        else
        {
            Monster thisMonster = PokemonList.Find(m => m.GetComponent<Monster>().Name.Equals(name)).GetComponent<Monster>();

            if (GameManager.instance.Team.Team.Find(m => m.Name.Equals(thisMonster.Name)))
            {
                transform.Find("Data/Pokemon").GetComponent<Image>().sprite = thisMonster.TextureFront;
                transform.Find("Data/Panel/Text").GetComponent<Text>().text = thisMonster.Info;
            }
            else
            {
                transform.Find("Data/Pokemon").GetComponent<Image>().sprite = thisMonster.TextureFront;
                transform.Find("Data/Panel/Text").GetComponent<Text>().text = "No info";
            }
        }
    }

    public void OnLeave(BaseEventData eventData)
    {
        transform.Find("Data/Name").GetComponent<Text>().text = "??????";
        transform.Find("Data/Pokemon").GetComponent<Image>().sprite = NoInfoSprite;
        transform.Find("Data/Panel/Text").GetComponent<Text>().text = "No info";

    }

    public static void AddEventTriggerListener(EventTrigger trigger,
                                            EventTriggerType eventType,
                                            System.Action<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(callback));
        trigger.triggers.Add(entry);
    }
}
