using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberDisplay : MonoBehaviour {

    [SerializeField] private Slider slider;
    [SerializeField] private Text levelText;
    [SerializeField] private Text expText;
    [SerializeField] private Image image;
    private Troop troop;

    private void OnDisable()
    {
        if (troop != null)
        {
            troop.LevelUp -= Troop_LevelUp;
            troop = null;
        }
    }

    public void AssignTroop(Troop t)
    {
        troop = t;
        slider.value = (float)troop.Experience / (float)troop.ExperienceNeededToLevel;
        levelText.text = "Level: " + troop.Level.ToString();
        image.sprite = troop.GetSprite();
        troop.LevelUp += Troop_LevelUp;
    }

    private void Troop_LevelUp(object sender, Troop.LevelUpArgs e)
    {
        levelText.text = "Level: " + troop.Level.ToString();
    }

    public void GiveExperienceToTroop(int exp)
    {
        StartCoroutine(GiveExperience(exp));
    }

    private IEnumerator GiveExperience(int exp)
    {
        expText.enabled = true;
        expText.text = "+" + exp.ToString() + " exp";
        yield return new WaitForSeconds(1.0f);


        while(exp > 0)
        {
            exp--;
            expText.text = expText.text = "+" + exp.ToString() + " exp";
            troop.GiveExperience(1);
            slider.value = (float)troop.Experience / (float)troop.ExperienceNeededToLevel;
            yield return null;
        }

        expText.text = expText.text = "+" + exp.ToString() + " exp";
        yield return new WaitForSeconds(1.0f);
        expText.enabled = false;
    }
}
