using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace BleSample.Core
{
    public class PermissionManager
    {
        /// <summary>
        /// Requests the permissions from the users
        /// </summary>
        /// <returns>The permissions and their status.</returns>
        /// <param name="permissions">Permissions to request.</param>
        public static async Task<Dictionary<Permission, PermissionStatus>> RequestPermissionsAsync(params Permission[] permissions)
        {
            var _permissionStatus = new Dictionary<Permission, PermissionStatus>();

            foreach (var permission in permissions)
            {
                try
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
                    if (status != PermissionStatus.Granted)
                    {
                        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(permission))
                        {
                            System.Diagnostics.Debug.WriteLine($"Need location. Gunna need that location");
                        }

                        var results = await CrossPermissions.Current.RequestPermissionsAsync(permission);
                        //Best practice to always check that the key exists
                        if (results.ContainsKey(permission))
                            status = results[permission];
                    }

                    if (status == PermissionStatus.Granted)
                    {
                        _permissionStatus.Add(permission, status);
                    }
                    //else if (status != PermissionStatus.Unknown)
                    //{
                    //    _permissionStatus.Add(permission, status);
                    //}
                    else
                    {
                        _permissionStatus.Add(permission, status);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    _permissionStatus.Add(permission, PermissionStatus.Unknown);
                    return _permissionStatus;
                }
            }

            return _permissionStatus;
        }

        /// <summary>
        /// Determines whether this instance has permission the specified permission.
        /// </summary>
        /// <returns><c>true</c> if this instance has permission the specified permission; otherwise, <c>false</c>.</returns>
        /// <param name="permission">Permission to check.</param>
        public static async Task<PermissionStatus> CheckPermissionStatusAsync(Permission permission)
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);

                return status;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return PermissionStatus.Unknown;
            }
        }
    }


}
