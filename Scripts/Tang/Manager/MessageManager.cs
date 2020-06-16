using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class MessageManager
    {
        // Token: 0x17000688 RID: 1672
		// (get) Token: 0x06003BC2 RID: 15298 RVA: 0x0010CCCA File Offset: 0x0010B0CA
		public static MessageManager Instance
		{
			get
			{
				if (MessageManager.m_instance == null)
				{
					MessageManager.m_instance = new MessageManager();
					MessageManager.m_instance.Init();
				}
				return MessageManager.m_instance;
			}
		}

		// Token: 0x06003BC3 RID: 15299 RVA: 0x0010CCEF File Offset: 0x0010B0EF
		public void Init()
		{
		}

		// Token: 0x06003BC4 RID: 15300 RVA: 0x0010CCF4 File Offset: 0x0010B0F4
		public void Subscribe(string mm, Action<object[]> task)
		{
			if (this.mmDic.ContainsKey(mm))
			{
				Dictionary<string, Action<object[]>> dictionary;
				(dictionary = this.mmDic)[mm] = (Action<object[]>)Delegate.Remove(dictionary[mm], task);
				(dictionary = this.mmDic)[mm] = (Action<object[]>)Delegate.Combine(dictionary[mm], task);
			}
			else
			{
				this.mmDic.Add(mm, task);
			}
		}

		// Token: 0x06003BC5 RID: 15301 RVA: 0x0010CD68 File Offset: 0x0010B168
		public void Unsubscribe(string mm, Action<object[]> task)
		{
			if (this.mmDic.ContainsKey(mm))
			{
				Dictionary<string, Action<object[]>> dictionary;
				(dictionary = this.mmDic)[mm] = (Action<object[]>)Delegate.Remove(dictionary[mm], task);
				if (this.mmDic[mm] == null)
				{
					this.mmDic.Remove(mm);
				}
			}
		}

		// Token: 0x06003BC6 RID: 15302 RVA: 0x0010CDC8 File Offset: 0x0010B1C8
		public void Dispatch(string mm, object[] data = null, DispatchType type = DispatchType.IMME, float delay = 2f)
		{
			if (type != DispatchType.IMME)
			{
				if (type != DispatchType.MANUAL)
				{
					if (type == DispatchType.TIMER)
					{
						if (this.mmDic.ContainsKey(mm))
						{
							this.mmDic[mm](data);
						}
						
//						Singleton<TaskRunner>.Instance.RunDelayTask(delay, true, delegate
//						{
//							if (this.mmDic.ContainsKey(mm))
//							{
//								this.mmDic[mm](data);
//							}
//						});
					}
				}
				else if (this.muDic.ContainsKey(mm))
				{
					this.muDic[mm] = data;
				}
				else
				{
					this.muDic.Add(mm, data);
				}
			}
			else
			{
				if (this.mmDic.ContainsKey(mm))
				{
					this.mmDic[mm](data);
					return;
				}
				if (this.log)
				{
					this.Log("MM:" + mm + " do not exist");
				}
			}
		}

		// Token: 0x06003BC7 RID: 15303 RVA: 0x0010CED4 File Offset: 0x0010B2D4
		public void PullsMM(string mm, Action<bool, object[]> data, bool autoRemove = true)
		{
			if (this.muDic.ContainsKey(mm))
			{
				data(true, this.muDic[mm]);
				if (autoRemove)
				{
					this.muDic.Remove(mm);
				}
			}
			else
			{
				data(false, null);
			}
		}

		// Token: 0x06003BC8 RID: 15304 RVA: 0x0010CF25 File Offset: 0x0010B325
		public void RemoveMM(string mm)
		{
			if (this.muDic.ContainsKey(mm))
			{
				this.muDic.Remove(mm);
			}
		}

		// Token: 0x06003BC9 RID: 15305 RVA: 0x0010CF45 File Offset: 0x0010B345
		public void ClearAll()
		{
			this.mmDic.Clear();
			this.muDic.Clear();
			MessageManager.m_instance = null;
		}

		// Token: 0x06003BCA RID: 15306 RVA: 0x0010CF63 File Offset: 0x0010B363
		private void Log(string str)
		{
			Debug.Log(str);
		}

		// Token: 0x06003BCB RID: 15307 RVA: 0x0010CF6B File Offset: 0x0010B36B
		public void ToggleLog()
		{
			this.log = !this.log;
		}

		// Token: 0x04002A4B RID: 10827
		private static MessageManager m_instance;

		// Token: 0x04002A4C RID: 10828
		private bool log;

		// Token: 0x04002A4D RID: 10829
		private Dictionary<string, Action<object[]>> mmDic = new Dictionary<string, Action<object[]>>(16);

		// Token: 0x04002A4E RID: 10830
		private Dictionary<string, object[]> muDic = new Dictionary<string, object[]>(4);
    }
    
    // Token: 0x020008BB RID: 2235
    public enum DispatchType
    {
	    // Token: 0x04002A48 RID: 10824
	    IMME,
	    // Token: 0x04002A49 RID: 10825
	    TIMER,
	    // Token: 0x04002A4A RID: 10826
	    MANUAL
    }

    public static class MessageName
    {
	    public static string SET_PLAYER1_VIGOR = "SET_PLAYER1_VIGOR";
	    public static string SET_PLAYER1_FinalHp = "SET_PLAYER1_FinalHp";
	    public static string SET_PLAYER1_Exp = "SET_PLAYER1_Exp";
	    public static string SET_PLAYER1_Level = "SET_PLAYER1_Level";
	    public static string SET_PLAYER1_Damage = "SET_PLAYER1_Damage";
	    public static string SET_PLAYER1_MovingSpeed = "SET_PLAYER1_MovingSpeed";
	    public static string SET_PLAYER1_FinalCritical = "SET_PLAYER1_FinalCritical"; 

	    public static string SET_PLAYER1_HP = "SET_PLAYER1_HP";
	    public static string SET_PLAYER1_MP = "SET_PLAYER1_MP";
	    public static string SET_PLAYER1_Money = "SET_PLAYER1_Money";
	    public static string SET_PLAYER1_Soul = "SET_PLAYER1_Soul";
	    public static string SET_PLAYER1_SoulIcon = "SET_PLAYER1_SoulIcon";
	    public static string SET_PLAYER1_ConsumableItemList = "SET_PLAYER1_ConsumableItemList";
	    public static string SET_PLAYER1_CurrConsumableItemIndex = "SET_PLAYER1_CurrConsumableItemIndex";
	    public static string SET_PLAYER1_Combo = "SET_PLAYER1_Combo";
	    public static string SET_PLAYER1_BOSS_HP = "SET_PLAYER1_BOSS_HP";
	    
	    public static string SHOW_BOSS_HP_UI = "SHOW_BOSS_HP_UI";
	    public static string HIDE_BOSS_HP_UI = "HIDE_BOSS_HP_UI";

	    public static string SHOW_OTHER_HP_UI = "SHOW_OTHER_HP_UI";
	    public static string HIDE_OTHER_HP_UI = "HIDE_OTHER_HP_UI";
	    public static string SET_OTHER_HP = "SET_OTHER_HP";

	    public static string SET_Monster_HP = "SET_Monster_HP";


    }
}