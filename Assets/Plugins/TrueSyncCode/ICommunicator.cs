using System;

public interface ICommunicator
{
    /// <summary>
    /// Round:���أ�Trip:��;
    /// ����pingʱ��
    /// </summary>
    /// <returns></returns>
	int RoundTripTime();

	void OpRaiseEvent(byte eventCode, object message, bool reliable, int[] toPlayers);

	void AddEventListener(OnEventReceived onEventReceived);
}
