using FishNet;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    private Dictionary<string, int> selectedJobs
        = new Dictionary<string, int>();

    private Dictionary<int, string> clientJobs
        = new Dictionary<int, string>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SelectJobServerRpc(
        string jobName,
        NetworkConnection sender = null)
    {
        if (selectedJobs.ContainsKey(jobName))
        {
            RejectJobClientRpc(sender, jobName);
            return;
        }

        selectedJobs[jobName] = sender.ClientId;
        clientJobs[sender.ClientId] = jobName;
        Debug.Log($"직업 선택됨: {jobName}");

        UpdateJobStatusClientRpc(jobName, true);
        ConfirmJobClientRpc(sender, jobName);
    }

    // 새로 접속한 클라이언트에게 현재 직업 상태 전송
    [ServerRpc(RequireOwnership = false)]
    public void RequestJobStatusServerRpc(
        NetworkConnection sender = null)
    {
        foreach (var job in selectedJobs)
        {
            UpdateJobStatusTargetRpc(sender, job.Key, true);
        }
    }

    [TargetRpc]
    void UpdateJobStatusTargetRpc(
        NetworkConnection conn, string jobName, bool isTaken)
    {
#if UNITY_ANDROID
        MobileJobSelect.Instance?.OnJobStatusUpdated(jobName, isTaken);
#endif
    }

    public void OnClientDisconnected(int clientId)
    {
        if (clientJobs.ContainsKey(clientId))
        {
            string jobName = clientJobs[clientId];
            selectedJobs.Remove(jobName);
            clientJobs.Remove(clientId);
            Debug.Log($"직업 해제됨: {jobName}");
            UpdateJobStatusClientRpc(jobName, false);
        }
    }

    [TargetRpc]
    void RejectJobClientRpc(
        NetworkConnection conn, string jobName)
    {
        Debug.Log($"직업 거부: {jobName}");
#if UNITY_ANDROID
        MobileJobSelect.Instance?.OnJobRejected(jobName);
#endif
    }

    [TargetRpc]
    void ConfirmJobClientRpc(
        NetworkConnection conn, string jobName)
    {
        Debug.Log($"직업 확인: {jobName}");
#if UNITY_ANDROID
        MobileJobSelect.Instance?.OnJobConfirmed(jobName);
#endif
    }

    [ObserversRpc]
    void UpdateJobStatusClientRpc(
        string jobName, bool isTaken)
    {
        Debug.Log($"직업 상태 업데이트: {jobName} = {isTaken}");
#if UNITY_ANDROID
        MobileJobSelect.Instance?.OnJobStatusUpdated(jobName, isTaken);
#endif
    }
}