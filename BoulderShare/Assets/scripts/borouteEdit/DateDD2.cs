using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DateDD2 : MonoBehaviour {
	public Dropdown year;
	public Dropdown month;
	public Dropdown day;
	private bool isLeapYear;
	private List<string> baseDays;
	private static int BaseYear = 2018;
	void Awake(){
		isLeapYear = false;

		baseDays = new List<string>();
		for(int i = 1 ; i < 29 ; i++){
			baseDays.Add(i+"");
		}

		DateTime now = DateTime.Now;
		SetYearOptions(now.Year);

		year.value = now.Year - BaseYear;
		month.value = now.Month - 1;
		day.value = now.Day - 1;
	}
	public void Init(){
		Awake();
	}
	//選択された日付をyyyyMMdd形式で返す
	public string GetDate(){
		return year.options[year.value].text +
			month.options[month.value].text.PadLeft(2, '0') +
			day.options[day.value].text.PadLeft(2, '0');
	}

	public void SetDate(string date){
		int v = int.Parse(date);

		SetDate(v);
	}

	public void SetDate(int v){
		day.value = (v % 100) - 1;
		v = (int) (v / 100.0f);
		month.value = (v % 100) - 1;
		v = (int) (v / 100.0f);
		year.value = v - BaseYear ;
	}
	//2018年から現在の年までのプルダウンを作成
	public void SetYearOptions(int currentYear){
		List<string> years = new List<string>();

		for(int i = BaseYear ; i <= currentYear ; i++){
			years.Add(i + "");
		}
		year.ClearOptions();
		year.AddOptions(years);
	}

	public void OnYearChanged(int value){
		//うるう年かどうかの判定
		int selected = int.Parse(year.options[value].text);

		if (selected % 4 == 0 && !(selected % 100 == 0 && selected % 400 != 0)){
			isLeapYear = true;
		}else{
			isLeapYear = false;
		}
	}

	public void OnMonthChanged(int value){
		List<string> days = new List<string>(baseDays);
		int type = -1;

		//月毎に日を決める
		if (value == 0 || value == 2 || value == 4 || value == 6 || value == 7
				|| value == 9 || value == 11){
			type = 0;
		}else if(value == 3 || value == 5 || value == 8 || value == 10){
			type = 1;
		}else{
			if (isLeapYear){
				type = 2;
			}else{
				type = 3;
			}
		}

		if (type == 0){
			days.Add("29");
			days.Add("30");
			days.Add("31");
		}else if (type == 1){
			days.Add("29");
			days.Add("30");
		}else if (type == 2){
			days.Add("29");
		}

		day.ClearOptions();
		day.AddOptions(days);
	}
}
