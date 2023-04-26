using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseActionUI : MonoBehaviour
{
    [SerializeField] private Transform fuseButtonPrefab;
    [SerializeField] private Transform fuseButtonContainerTransform;

    [SerializeField] private GameObject[] summons;

    private List<FuseButtonUI> fuseButtonUIList;
    //Creates list for actionbuttons
    private void Awake() 
    {
        fuseButtonUIList = new List<FuseButtonUI>();
        ClearButtons();
    }

    public void ClearButtons()
    {
        if (!fuseButtonContainerTransform) {return;}
        foreach (Transform buttonTransform in fuseButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        fuseButtonUIList.Clear();
    }

    public void CreateFuseButtons(int boneNumber)
    {
        ClearButtons();
        if (!fuseButtonContainerTransform) {return;}
        for(int i = 1; i <= summons.Length; i++)
        {
            if (boneNumber < i) {continue;}

            Transform fuseButtonTransform = Instantiate(fuseButtonPrefab, fuseButtonContainerTransform);
            FuseButtonUI fuseButtonUI = fuseButtonTransform.GetComponent<FuseButtonUI>();
            fuseButtonUI.SetSummon(summons[i - 1], i);

            fuseButtonUIList.Add(fuseButtonUI);
        }
        
    }
}
