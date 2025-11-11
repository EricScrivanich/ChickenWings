using UnityEngine;

public interface INavigationUI
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  public GameObject GetFirstSelected();
  public GameObject GetNextSelected();
}
