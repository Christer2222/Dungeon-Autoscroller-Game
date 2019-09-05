using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpScreen : MonoBehaviour
{
	public GameObject levelUpScreen;

	private CombatController playerCombatController;

	private int strengthChange, dexterityChange, intelligenceChange, luckChange;
	private int traitPointsToSpend = 5, abilityPointsToSpend;

	private Text strengthText, dexterityText, intelligenceText, luckText;
	private Text traitPointToSpendText, availableAbilitiesText;
	private Text classText;

	private Button strengthPlusButton, dexterityPlusButton, intelligencePlusButton, luckPlusButton;
	private Button strengthMinusButton, dexterityMinusButton, intelligenceMinusButton, luckMinusButton;
	private Button changeClassButton;
	private Button abilityPickButton1, abilityPickButton2, abilityPickButton3;
	private Button cancelButton, confirmButton;

	void Start()
	{
		GetComponent<Button>().onClick.AddListener(delegate { ToggleLevelUpScreen(); });

		playerCombatController = GameObject.Find("$Player").GetComponent<CombatController>();


		var _UICanvas = GameObject.Find("$UICanvas");
		foreach(Transform _t in _UICanvas.GetComponentsInChildren<Transform>(true))
		{
			switch(_t.name)
			{
				case "$LevelUpHolder":
					levelUpScreen = _t.gameObject;
					break;
				case "$StrengthCurrentTraitText":
					strengthText = _t.GetComponent<Text>();
					break;
				case "$DexterityCurrentTraitText":
					dexterityText = _t.GetComponent<Text>();
					break;
				case "$IntelligenceCurrentTraitText":
					intelligenceText = _t.GetComponent<Text>();
					break;
				case "$LuckCurrentTraitText":
					luckText = _t.GetComponent<Text>();
					break;
				case "$StrengthPlusButton":
					strengthPlusButton = _t.GetComponent<Button>();
					strengthPlusButton.onClick.AddListener(delegate {
						UpdateTraitText(ref strengthChange,1,playerCombatController.myStats.baseStrength,ref strengthText,ref strengthMinusButton);
					});
					break;
				case "$DexterityPlusButton":
					dexterityPlusButton = _t.GetComponent<Button>();
					dexterityPlusButton.onClick.AddListener(delegate {
						UpdateTraitText(ref dexterityChange,1,playerCombatController.myStats.baseDexterity,ref dexterityText, ref dexterityMinusButton);
					});
					break;
				case "$IntelligencePlusButton":
					intelligencePlusButton = _t.GetComponent<Button>();
					intelligencePlusButton.onClick.AddListener(delegate {
						UpdateTraitText(ref intelligenceChange,1,playerCombatController.myStats.baseIntelligence,ref intelligenceText, ref intelligenceMinusButton);
					});
					break;
				case "$LuckPlusButton":
					luckPlusButton = _t.GetComponent<Button>();
					luckPlusButton.onClick.AddListener(delegate {
						UpdateTraitText(ref luckChange,1,playerCombatController.myStats.baseLuck,ref luckText, ref luckMinusButton);
					});
					break;
				case "$StrengthMinusButton":
					strengthMinusButton = _t.GetComponent<Button>();
					strengthMinusButton.onClick.AddListener(delegate {
						UpdateTraitText(ref strengthChange,-1,playerCombatController.myStats.baseStrength,ref strengthText, ref strengthMinusButton);
					});
					break;
				case "$DexterityMinusButton":
					dexterityMinusButton = _t.GetComponent<Button>();
					dexterityMinusButton.onClick.AddListener(delegate
					{
						UpdateTraitText(ref dexterityChange,-1,playerCombatController.myStats.baseDexterity,ref dexterityText, ref dexterityMinusButton);
					});
					break;
				case "$IntelligenceMinusButton":
					intelligenceMinusButton = _t.GetComponent<Button>();
					intelligenceMinusButton.onClick.AddListener(delegate
					{
						UpdateTraitText(ref intelligenceChange,-1,playerCombatController.myStats.baseIntelligence,ref intelligenceText, ref intelligenceMinusButton);
					});
					break;
				case "$LuckMinusButton":
					luckMinusButton = _t.GetComponent<Button>();
					luckMinusButton.onClick.AddListener(delegate
					{
						UpdateTraitText(ref luckChange,-1,playerCombatController.myStats.baseLuck,ref luckText, ref luckMinusButton);
					});
					break;
				case "$ClassChangePossibleButton":
					changeClassButton = _t.GetComponent<Button>();
					break;
				case "$ClassText":
					classText = _t.GetComponent<Text>();
					break;
				case "$AvailableAbilitiesText":
					availableAbilitiesText = _t.GetComponent<Text>();
					break;
				case "$AbilityButton1":
					abilityPickButton1 = _t.GetComponent<Button>();
					break;
				case "$AbilityButton2":
					abilityPickButton2 = _t.GetComponent<Button>();
					break;
				case "$AbilityButton3":
					abilityPickButton3 = _t.GetComponent<Button>();
					break;
				case "$CancelButton":
					cancelButton = _t.GetComponent<Button>();
					cancelButton.onClick.AddListener(delegate
					{
						traitPointsToSpend = strengthChange + dexterityChange + intelligenceChange + luckChange;
						strengthChange = dexterityChange = intelligenceChange = luckChange = 0;
					});

					break;
				case "$ConfirmButton":
					confirmButton = _t.GetComponent<Button>();
					break;
			}
		}
	}

	public void UpdateTraitText(ref int _change, int _difference, int _traitScore, ref Text _textToChange, ref Button _minusButton)
	{
		_change += _difference;
		traitPointsToSpend -= _difference;
		_minusButton.gameObject.SetActive(_change > 0);
		//_plusButton.gameObject.SetActive(traitPointsToSpend > 0);

		strengthPlusButton.gameObject.SetActive(traitPointsToSpend > 0);
		dexterityPlusButton.gameObject.SetActive(traitPointsToSpend > 0);
		intelligencePlusButton.gameObject.SetActive(traitPointsToSpend > 0);
		luckPlusButton.gameObject.SetActive(traitPointsToSpend > 0);

		//strengthMinusButton.gameObject.SetActive(_change > 0);
		///dexterityMinusButton.gameObject.SetActive(_change > 0);
		//intelligenceMinusButton.gameObject.SetActive(_change > 0);
		//luckMinusButton.gameObject.SetActive(_change > 0);


		_textToChange.text = (_traitScore + _change) + ((_change > 0)? "(+" + _change + ")": "");
	}


	public void ToggleLevelUpScreen()
	{
		levelUpScreen.SetActive(!levelUpScreen.activeSelf);

		if (levelUpScreen.activeSelf)
		{
			strengthText.text = playerCombatController.myStats.baseStrength.ToString();
			dexterityText.text = playerCombatController.myStats.baseDexterity.ToString();
			intelligenceText.text = playerCombatController.myStats.baseIntelligence.ToString();
			luckText.text = playerCombatController.myStats.baseLuck.ToString();

			bool _hasTraitPoints = (traitPointsToSpend > 0);
			strengthPlusButton.gameObject.SetActive(_hasTraitPoints);
			dexterityPlusButton.gameObject.SetActive(_hasTraitPoints);
			intelligencePlusButton.gameObject.SetActive(_hasTraitPoints);
			luckPlusButton.gameObject.SetActive(_hasTraitPoints);

			strengthMinusButton.gameObject.SetActive(false);
			dexterityMinusButton.gameObject.SetActive(false);
			intelligenceMinusButton.gameObject.SetActive(false);
			luckMinusButton.gameObject.SetActive(false);

		}
	}
}
