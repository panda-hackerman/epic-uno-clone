using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deck", menuName = "ScriptableObjects/CustomDeck", order = 1)]
public class UneDeck : ScriptableObject
{
    public List<GameObject> cards = new List<GameObject>(new GameObject[54]);
}