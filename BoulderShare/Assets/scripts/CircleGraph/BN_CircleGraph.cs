using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

namespace BoulderNotes{
public class BN_CircleGraph : MonoBehaviour{
    [SerializeField] private RectTransform rootOfItems;
    [SerializeField] private BNCGItem itemPrefab;
    [SerializeField] private BNCGProperty[] itemProperty;
    private float diameter;
    private List<BNCGItem> items;

    public void Init(){
        foreach(Transform t in rootOfItems){
            Destroy(t.gameObject);
        }
        diameter = Mathf.Min(rootOfItems.rect.width, rootOfItems.rect.height);

        items = new List<BNCGItem>();
        int priority = 0;
        foreach(BNCGProperty p in itemProperty){
            BNCGItem item = Instantiate<BNCGItem>(itemPrefab, rootOfItems);
            item.Init(diameter, p.color, priority);
            items.Add(item);
            priority++;
        }
    }
    public void Make(int[] counts){
        float[] rates;
        int totalAmounts = counts.Sum(x => x);
        if (totalAmounts == 0){
            return ;
        }
        rates = counts.Select(x => 1f * x / totalAmounts).ToArray();

        Make(rates);
    }
    public void Make(float[] rates){
        for(int i = 0 ; i < rates.Length ; i++){
            items[i].SetAmount(rates[i]);
        }

        IEnumerable<BNCGItem> ite = items.OrderBy(x => x.GetAmount()).ThenByDescending(x => x.GetPriority());

        float degRate = 0.0f;
        foreach(BNCGItem item in ite){
            if (item.GetAmount() == 0.0f){
                item.DeActive();
                continue;
            }
            degRate += item.GetAmount();
            item.Arrenge(degRate);
            item.Active();

        }
    }
}
[Serializable]
public class BNCGProperty{
    public Color color;
}
}