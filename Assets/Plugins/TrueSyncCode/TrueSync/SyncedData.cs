using System;
using System.Collections.Generic;

namespace TrueSync
{
	[Serializable]
	public class SyncedData : ResourcePoolItem//ResourcePoolItem�ӿ�ʵ��cleanup����
	{
		internal static ResourcePoolSyncedData pool = new ResourcePoolSyncedData();//SyncedData

		internal static ResourcePoolListSyncedData poolList = new ResourcePoolListSyncedData();//List<SyncedData>

		public InputDataBase inputData;//�������룬�������ֵ�ķ�ʽ�������ݣ������������л��ͷ����л�

		public int tick;

		[NonSerialized]
		public bool fake;

		[NonSerialized]
		public bool dirty;

		[NonSerialized]
		public bool dropPlayer;//������ң�

		[NonSerialized]
		public byte dropFromPlayerId;

		private static List<byte> bytesToEncode = new List<byte>();

		public SyncedData()
		{
			this.inputData = AbstractLockstep.instance.InputDataProvider();
		}
        /// <summary>
        /// ��ʼ���������������ĸ����,�����ĸ�tickʱ�������
        /// </summary>
        /// <param name="ownerID"></param>
        /// <param name="tick"></param>
		public void Init(byte ownerID, int tick)
		{
			this.inputData.ownerID = ownerID;
			this.tick = tick;
			this.fake = false;
			this.dirty = false;
		}

        /// <summary>
        /// ���л�ͷ���� ��tick ownerID dropFormPlayerId dropPlayer���뵽List<Byte>
        /// </summary>
        /// <param name="bytes"></param>
		public void GetEncodedHeader(List<byte> bytes)
		{
			Utils.GetBytes(tick, bytes);
			bytes.Add(inputData.ownerID);
			bytes.Add(dropFromPlayerId);
			bytes.Add((byte)(dropPlayer ? 1 : 0));
		}

        /// <summary>
        /// ���л� ������������ʵ�������л��ֵ䣩
        /// </summary>
        /// <param name="bytes"></param>
		public void GetEncodedActions(List<byte> bytes)
		{
			this.inputData.Serialize(bytes);
		}

		public static List<SyncedData> Decode(byte[] data)
		{
			List<SyncedData> @new = poolList.GetNew();
			@new.Clear();
			int i = 0;
			int num = BitConverter.ToInt32(data, i);
			i += 4;
			byte ownerID = data[i++];
			byte b = data[i++];
			bool flag = data[i++] == 1;
			int num2 = num;
			while (i < data.Length)
			{
				SyncedData new2 = SyncedData.pool.GetNew();
				new2.Init(ownerID, num2--);
				new2.inputData.Deserialize(data, ref i);
				@new.Add(new2);
			}
			bool flag2 = @new.Count > 0;
			if (flag2)
			{
				@new[0].dropPlayer = flag;
				@new[0].dropFromPlayerId = b;
			}
			return @new;
		}

        /// <summary>
        /// ���л�һ��tick����������
        /// </summary>
        /// <param name="syncedData"></param>
        /// <returns></returns>
		public static byte[] Encode(SyncedData[] syncedData)
		{
			SyncedData.bytesToEncode.Clear();
			if (syncedData.Length != 0)
			{
				syncedData[0].GetEncodedHeader(SyncedData.bytesToEncode);//tick ���id
				for (int i = 0; i < syncedData.Length; i++)
				{
					syncedData[i].GetEncodedActions(SyncedData.bytesToEncode);//��tick�µ����в���
				}
			}
            //����һ���µ��ֽ�����array����
			byte[] array = new byte[SyncedData.bytesToEncode.Count];
			int j = 0;
			int num = array.Length;
			while (j < num)
			{
				array[j] = SyncedData.bytesToEncode[j];
				j++;
			}
			return array;
		}

		public SyncedData clone()
		{
			SyncedData @new = SyncedData.pool.GetNew();
			@new.Init(this.inputData.ownerID, this.tick);
			@new.inputData.CopyFrom(this.inputData);
			return @new;
		}

		public bool EqualsData(SyncedData other)
		{
			return this.inputData.EqualsData(other.inputData);
		}

		public void CleanUp()
		{
			this.inputData.CleanUp();
		}
	}
}
