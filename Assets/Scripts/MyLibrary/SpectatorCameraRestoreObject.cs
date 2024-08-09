using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyLibrary
{
    public class SpectatorCameraRestoreObject
{
    SpectatorCameraRestoreObjectProperties spectatorCameraRestoreObjectProperties;

    public SpectatorCameraRestoreObject(SpectatorCameraRestoreObjectProperties spectatorCameraRestoreObjectProperties)
    {
        this.spectatorCameraRestoreObjectProperties = spectatorCameraRestoreObjectProperties;
    }

    public string ConstructRestoreObject(string name)
    {
        spectatorCameraRestoreObjectProperties.gridPropertiesList = spectatorCameraRestoreObjectProperties.gridPropertiesList.OrderBy(item => item.name).ToList();
        GridProperties gridProperties = FindByName(name);
        if(gridProperties == null)
        {
            return FindGrid(name);
        }
        else
        {
            if(gridProperties.amount >= 10)
            {
                return FindGrid(name);
            }

            gridProperties.amount++;
            
        }
        return "Got it!";
    }

    string FindGrid(string name)
    {
        var firstInvalidItem = spectatorCameraRestoreObjectProperties.gridPropertiesList.FirstOrDefault(item => string.IsNullOrEmpty(item.name));

        if (firstInvalidItem == null)
            return "Ooops! There are no more empty cells.";

        firstInvalidItem.name = name;
        firstInvalidItem.amount++;
        return "Luckily there is still a slot left.";
    }

    public GridProperties FindByName(string name)
    {
        // Tìm kiếm bằng tìm kiếm nhị phân
        int index = spectatorCameraRestoreObjectProperties.gridPropertiesList.BinarySearch(new GridProperties { name = name }, new GridPropertiesComparer());
        if (index >= 0)
        {
            return spectatorCameraRestoreObjectProperties.gridPropertiesList[index];
        }
        else
        {
            return null; // Hoặc xử lý khi không tìm thấy
        }
    }
}

[System.Serializable]
public class SpectatorCameraRestoreObjectProperties
{
    public List<GridProperties> gridPropertiesList; 
}
}

