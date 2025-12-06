using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INativeShare
{
    void ShareText(string message, string shareDialogTitle);
    void ShareSingleFile(string path, string fileType, string message, string shareDialogTitle);
    void ShareMultipleFileOfSameType(string[] path, string fileType, string message, string shareDialogTitle);
	void ShareMultipleFileOfMultipleType(string[] path, string message, string shareDialogTitle);
}
