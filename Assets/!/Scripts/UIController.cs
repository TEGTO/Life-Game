using GameControlling;
using SoundNS;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UINS
{
	public class UIController : MonoBehaviour
	{
		private const string CURRENT_POPULATION_TEXT = "Current Population: ";

		[SerializeField]
		private UIDocument uIDocument;
		[SerializeField]
		private GameCommand gameCommand;
		[SerializeField]
		private int startedCellsAmountAndBoundaries = 100;

		private Button restartCameraButton;
		private Button cameraZoomUpButton;
		private Button cameraZoomDownButton;
		private Button restartButton;
		private Button playButton;
		private Button pauseButton;
		private Label speedLabel;
		private Label currentPopulation;
		private Button speedUpButton;
		private Button speedDownButton;
		private TextField seedText;
		private Button enterSeed;
		private Button randomSeed;

		private MinMaxSlider cellAmountSlider;
		private TextField cellAmountTextMin;
		private TextField cellAmountTextMax;
		private MinMaxSlider spawnBoundariesSlider;
		private TextField spawnBoundariesTextMin;
		private TextField spawnBoundariesTextMax;

		private void Start()
		{
			InitializeUIElements();
		}
		private void InitializeUIElements()
		{
			var root = uIDocument.rootVisualElement;
			restartCameraButton = root.Q<Button>("ResetCameraButton");
			cameraZoomUpButton = root.Q<Button>("CameraZoomUpButton");
			cameraZoomDownButton = root.Q<Button>("CameraZoomDownButton");
			restartButton = root.Q<Button>("ResetButton");
			playButton = root.Q<Button>("PlayButton");
			pauseButton = root.Q<Button>("PauseButton");
			speedLabel = root.Q<Label>("SpeedLabel");
			currentPopulation = root.Q<Label>("CurrentPopulationLabel");
			speedUpButton = root.Q<Button>("SpeedUpButton");
			speedDownButton = root.Q<Button>("SpeedDownButton");
			seedText = root.Q<TextField>("SeedText");
			enterSeed = root.Q<Button>("EnterSeedButton");
			randomSeed = root.Q<Button>("RandomSeedButton");

			cellAmountSlider = root.Q<MinMaxSlider>("CellAmountSlider");
			cellAmountTextMin = root.Q<TextField>("CellAmountTextMin");
			cellAmountTextMax = root.Q<TextField>("CellAmountTextMax");
			spawnBoundariesSlider = root.Q<MinMaxSlider>("SpawnBoundariesSlider");
			spawnBoundariesTextMin = root.Q<TextField>("SpawnBoundariesTextMin");
			spawnBoundariesTextMax = root.Q<TextField>("SpawnBoundariesTextMax");

			RegisterSliderAndText(cellAmountTextMin, cellAmountTextMax, cellAmountSlider, gameCommand.ChangeCellSpawnAmount);
			RegisterSliderAndText(spawnBoundariesTextMin, spawnBoundariesTextMax, spawnBoundariesSlider, gameCommand.ChangeCellSpawnBoundaries);

			RegisterButtonClickEvent(restartCameraButton, ResetCamera);
			RegisterButtonClickEvent(cameraZoomUpButton, CameraZoomUp);
			RegisterButtonClickEvent(cameraZoomDownButton, CameraZoomDown);
			RegisterButtonClickEvent(restartButton, ResetGame);
			RegisterButtonClickEvent(playButton, PauseGame);
			RegisterButtonClickEvent(pauseButton, PlayGame);
			RegisterButtonClickEvent(speedUpButton, UpGameSpeed);
			RegisterButtonClickEvent(speedDownButton, DownGameSpeed);
			RegisterButtonClickEvent(enterSeed, EnterNewSeed);
			RegisterButtonClickEvent(randomSeed, GenerateRandomSeed);
			RegisterCanUseUIButtonEvent(speedLabel);
			RegisterCanUseUIButtonEvent(currentPopulation);
			RegisterSeedText(seedText);
			GenerateRandomSeed();
		}
		private void LateUpdate()
		{
			currentPopulation.text = CURRENT_POPULATION_TEXT + gameCommand.CurrentPopulation.ToString();
		}
		private void RegisterSeedText(TextField seedText)
		{
			RegisterCanUseUIButtonEvent(seedText);
			seedText.value = gameCommand.SeedName;
		}
		private void RegisterButtonClickEvent(Button button, Action clickEvent)
		{
			RegisterButtonClickSound(button);
			RegisterCanUseUIButtonEvent(button);
			button.RegisterCallback<ClickEvent>(
			   e =>
			   {
				   clickEvent();
			   },
			   TrickleDown.TrickleDown);
		}
		private void RegisterButtonClickSound(Button button)
		{
			button.RegisterCallback<ClickEvent>(
			 e =>
			 {
				 SoundManager.Instance.PlayUIButtonClick();
			 },
			 TrickleDown.TrickleDown);
		}
		private void RegisterCanUseUIButtonEvent(VisualElement ve)
		{
			ve.RegisterCallback<MouseEnterEvent>(
				e =>
				{
					gameCommand.SetCanUseUIButtonEventStatus(false);
				}, TrickleDown.TrickleDown);
			ve.RegisterCallback<MouseLeaveEvent>(
				e =>
				{
					gameCommand.SetCanUseUIButtonEventStatus(true);
				}, TrickleDown.TrickleDown);
		}
		private void RegisterSliderAndText(TextField textFieldMin, TextField textFieldMax, MinMaxSlider slider, Action<float, float> func)
		{
			RegisterCanUseUIButtonEvent(textFieldMin);
			RegisterCanUseUIButtonEvent(textFieldMax);
			RegisterCanUseUIButtonEvent(slider);
			slider.minValue = 0;
			slider.maxValue = startedCellsAmountAndBoundaries;
			textFieldMin.value = slider.minValue.ToString();
			textFieldMax.value = slider.maxValue.ToString();
			slider.RegisterValueChangedCallback(
				e =>
				{
					textFieldMin.value = ((int)slider.minValue).ToString();
					textFieldMax.value = ((int)slider.maxValue).ToString();
					func(slider.minValue, slider.maxValue);
					SoundManager.Instance.PlayUISliderChange();
				}
				);
			textFieldMin.RegisterValueChangedCallback(
				e =>
				{
					TextFieldOnlyDigitsCheck(e);
					if (float.TryParse(textFieldMin.value, out float floatValue))
						slider.minValue = floatValue;
					textFieldMin.value = slider.minValue.ToString();
				}
				);
			textFieldMax.RegisterValueChangedCallback(
				e =>
				{
					TextFieldOnlyDigitsCheck(e);
					if (float.TryParse(textFieldMax.value, out float floatValue))
						slider.maxValue = floatValue;
					textFieldMax.value = slider.maxValue.ToString();
				}
				);
			func(slider.minValue, slider.maxValue);
		}
		private void TextFieldOnlyDigitsCheck(ChangeEvent<string> e)
		{
			var textField = e.target as TextField;
			if (!ValidateCharacters(e.newValue, "0123456789"))
				textField.value = e.previousValue;
			if (textField.value.Length <= 0)
				textField.value = "0";
		}
		private void ResetCamera()
		{
			gameCommand.CameraReset();
		}
		private void CameraZoomUp()
		{
			gameCommand.CameraZoomUp();
		}
		private void CameraZoomDown()
		{
			gameCommand.CameraZoomDown();
		}
		private void UpGameSpeed()
		{
			gameCommand.GameSpeedUp();
			UpdateSpeedLabel();
		}
		private void DownGameSpeed()
		{
			gameCommand.GameSpeedDown();
			UpdateSpeedLabel();
		}
		private void UpdateSpeedLabel()
		{
			speedLabel.text = gameCommand.GameSpeed.ToString();
		}
		private void PauseGame()
		{
			playButton.style.display = DisplayStyle.None;
			pauseButton.style.display = DisplayStyle.Flex;
			gameCommand.PauseGame();
		}
		private void PlayGame()
		{
			pauseButton.style.display = DisplayStyle.None;
			playButton.style.display = DisplayStyle.Flex;
			gameCommand.PlayGame();
		}
		private void ResetGame()
		{
			PauseGame();
			gameCommand.RestartGame();
		}
		private void EnterNewSeed()
		{
			PauseGame();
			gameCommand.EnterNewSeed(seedText.text);
		}
		private void GenerateRandomSeed()
		{
			PauseGame();
			gameCommand.GenerateNewSeed();
			seedText.value = gameCommand.SeedName;
		}
		bool ValidateCharacters(string value, string validCharacters)
		{
			foreach (var c in value)
			{
				if (!validCharacters.Contains(c))
					return false;
			}
			return true;
		}
	}
}