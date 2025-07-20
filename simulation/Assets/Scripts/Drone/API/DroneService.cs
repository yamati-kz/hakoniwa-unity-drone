using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace hakoniwa.drone.service
{
    public static class DroneServiceRC
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const string DllName = "hako_service_c"; // Windows
#else
        private const string DllName = "libhako_service_c"; // Ubuntu, Mac
#endif
        /*
         * Initialization and Control
         */
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_init(int enableDatalog, string droneConfigDirPath, string debugLogPath);

        public static int Init(string droneConfigDirPath)
        {
            return Init(droneConfigDirPath, null, 0);
        }

        public static int Init(string droneConfigDirPath, string debugLogPath)
        {
            return Init(droneConfigDirPath, debugLogPath, 0);
        }
        public static int Init(string droneConfigDirPath, string debugLogPath, int enableDatalog)
        {
            try
            {
                return drone_service_rc_init(enableDatalog, droneConfigDirPath, debugLogPath);
            }
            catch (DllNotFoundException e)
            {
                Debug.LogError($"DllNotFoundException: {e.Message}");
                return -1;
            }
            catch (EntryPointNotFoundException e)
            {
                Debug.LogError($"EntryPointNotFoundException: {e.Message}");
                return -1;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Exception: {e.Message}");
                return -1;
            }
        }
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_init_single(string drone_config_text, string controller_config_text, int logger_enable, string debug_logpath);
        public static int InitSingle(string droneConfigText, string controllerConfigText, bool loggerEnable, string debug_logpath)
        {
            try
            {
                int loggerFlag = loggerEnable ? 1 : 0;
                return drone_service_rc_init_single(droneConfigText, controllerConfigText, loggerFlag, debug_logpath);
            }
            catch (DllNotFoundException e)
            {
                Debug.LogError($"DllNotFoundException: {e.Message}");
                return -1;
            }
            catch (EntryPointNotFoundException e)
            {
                Debug.LogError($"EntryPointNotFoundException: {e.Message}");
                return -1;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Exception: {e.Message}");
                return -1;
            }
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_start();

        public static int Start()
        {
            return drone_service_rc_start();
        }
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_set_debuff_on_collision(int index, int debuff_duration_msec);
        public static int SetDebuffOnCollision(int index, int debuffDurationMsec)
        {
            try
            {
                return drone_service_set_debuff_on_collision(index, debuffDurationMsec);
            }
            catch (DllNotFoundException e)
            {
                Debug.LogError($"DllNotFoundException: {e.Message}");
                return -1;
            }
            catch (EntryPointNotFoundException e)
            {
                Debug.LogError($"EntryPointNotFoundException: {e.Message}");
                return -1;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Exception: {e.Message}");
                return -1;
            }
        }


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_run();

        public static int Run()
        {
            return drone_service_rc_run();
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_stop();

        public static int Stop()
        {
            return drone_service_rc_stop();
        }

        /*
         * Stick Operations
         */
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_put_vertical(int index, double value);

        public static int PutVertical(int index, double value)
        {
            return drone_service_rc_put_vertical(index, value);
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_put_horizontal(int index, double value);

        public static int PutHorizontal(int index, double value)
        {
            return drone_service_rc_put_horizontal(index, value);
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_put_heading(int index, double value);

        public static int PutHeading(int index, double value)
        {
            return drone_service_rc_put_heading(index, value);
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_put_forward(int index, double value);

        public static int PutForward(int index, double value)
        {
            return drone_service_rc_put_forward(index, value);
        }

        /*
         * Button Operations
         */
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_put_radio_control_button(int index, int value);

        public static int PutRadioControlButton(int index, int value)
        {
            return drone_service_rc_put_radio_control_button(index, value);
        }
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_put_mode_change_button(int index, int value);

        public static int PutModeChangeButton(int index, int value)
        {
            return drone_service_rc_put_mode_change_button(index, value);
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_put_magnet_control_button(int index, int value);

        public static int PutMagnetControlButton(int index, int value)
        {
            return drone_service_rc_put_magnet_control_button(index, value);
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_put_camera_control_button(int index, int value);

        public static int PutCameraControlButton(int index, int value)
        {
            return drone_service_rc_put_camera_control_button(index, value);
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_put_home_control_button(int index, int value);

        public static int PutHomeControlButton(int index, int value)
        {
            return drone_service_rc_put_home_control_button(index, value);
        }
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_get_flight_mode(int index, out int mode);

        public static int GetFlightMode(int index, out int mode)
        {
            return drone_service_rc_get_flight_mode(index, out mode);
        }
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_get_internal_state(int index, out int state);

        public static int GetInternalState(int index, out int state)
        {
            return drone_service_rc_get_internal_state(index, out state);
        }
        /*
         * Get Position and Attitude
         */
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_get_position(int index, out double x, out double y, out double z);

        public static int GetPosition(int index, out double x, out double y, out double z)
        {
            return drone_service_rc_get_position(index, out x, out y, out z);
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_get_attitude(int index, out double x, out double y, out double z);

        public static int GetAttitude(int index, out double x, out double y, out double z)
        {
            return drone_service_rc_get_attitude(index, out x, out y, out z);
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_get_controls(int index, out double c1, out double c2, out double c3, out double c4, out double c5, out double c6, out double c7, out double c8);

        public static int GetControls(int index, out double c1, out double c2, out double c3, out double c4, out double c5, out double c6, out double c7, out double c8)
        {
            return drone_service_rc_get_controls(index, out c1, out c2, out c3, out c4, out c5, out c6, out c7, out c8);
        }
        /*
         * Battery
         */
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_get_battery_status(
            int index,
            out double fullVoltage,
            out double currVoltage,
            out double currTemp,
            out uint status,
            out uint cycles);

        public struct BatteryStatus
        {
            public double FullVoltage;
            public double CurrentVoltage;
            public double CurrentTemperature;
            public uint Status;
            public uint ChargeCycles;
        }

        public static bool TryGetBatteryStatus(int index, out BatteryStatus batteryStatus)
        {
            batteryStatus = new BatteryStatus();
            try
            {
                int result = drone_service_rc_get_battery_status(index, out batteryStatus.FullVoltage, out batteryStatus.CurrentVoltage, out batteryStatus.CurrentTemperature, out batteryStatus.Status, out batteryStatus.ChargeCycles);
                return result == 0;
            }
            catch (DllNotFoundException e)
            {
                Debug.LogError($"DllNotFoundException: {e.Message}");
            }
            catch (EntryPointNotFoundException e)
            {
                Debug.LogError($"EntryPointNotFoundException: {e.Message}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Exception: {e.Message}");
            }
            return false;
        }
        /*
         * Disturbance
         */
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_put_disturbance(int index, double d_temp, double d_wind_x, double d_wind_y, double d_wind_z);

        public static int PutDisturbance(int index, double temp, double windX, double windY, double windZ)
        {
            return drone_service_rc_put_disturbance(index, temp, windX, windY, windZ);
        }

        /*
         * Collision
         */
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_put_collision(int index, double contact_position_x, double contact_position_y, double contact_position_z, double restitution_coefficient);

        public static int PutCollision(int index, double contactPositionX, double contactPositionY, double contactPositionZ, double restitutionCoefficient)
        {
            return drone_service_rc_put_collision(index, contactPositionX, contactPositionY, contactPositionZ, restitutionCoefficient);
        }
        /*
         * Collision Impulse
         */
        [StructLayout(LayoutKind.Sequential)]
        public struct HakoVectorType
        {
            public double x;
            public double y;
            public double z;
        }
        private static HakoVectorType ConvertToHakoVectorType(Vector3 vector)
        {
            return new HakoVectorType
            {
                x = vector.x,
                y = vector.y,
                z = vector.z
            };
        }
        public static int PutImpulseByCollision(
            int index,
            bool isTargetStatic,
            Vector3 targetVelocity,
            Vector3 targetAngularVelocity,
            Vector3 targetEuler,
            Vector3 selfContactVector,
            Vector3 targetContactVector,
            Vector3 targetInertia,
            Vector3 normal,
            double targetMass,
            double restitutionCoefficient)
        {
            HakoVectorType hakoTargetVelocity = ConvertToHakoVectorType(targetVelocity);
            HakoVectorType hakoTargetAngularVelocity = ConvertToHakoVectorType(targetAngularVelocity);
            HakoVectorType hakoTargetEuler = ConvertToHakoVectorType(targetEuler);
            HakoVectorType hakoSelfContactVector = ConvertToHakoVectorType(selfContactVector);
            HakoVectorType hakoTargetContactVector = ConvertToHakoVectorType(targetContactVector);
            HakoVectorType hakoTargetInertia = ConvertToHakoVectorType(targetInertia);
            HakoVectorType hakoNormal = ConvertToHakoVectorType(normal);

            // 既存のメソッド呼び出し
            return OriginalPutImpulseByCollision(
                index,
                isTargetStatic,
                hakoTargetVelocity,
                hakoTargetAngularVelocity,
                hakoTargetEuler,
                hakoSelfContactVector,
                hakoTargetContactVector,
                hakoTargetInertia,
                hakoNormal,
                targetMass,
                restitutionCoefficient);
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_put_impulse_by_collision(
            int index,
            int is_target_static,
            HakoVectorType target_velocity,
            HakoVectorType target_angular_velocity,
            HakoVectorType target_euler,
            HakoVectorType self_contact_vector,
            HakoVectorType target_contact_vector,
            HakoVectorType target_inertia,
            HakoVectorType normal,
            double target_mass,
            double restitution_coefficient);

        public static int OriginalPutImpulseByCollision(
            int index,
            bool isTargetStatic,
            HakoVectorType targetVelocity,
            HakoVectorType targetAngularVelocity,
            HakoVectorType target_euler,
            HakoVectorType selfContactVector,
            HakoVectorType targetContactVector,
            HakoVectorType targetInertia,
            HakoVectorType normal,
            double targetMass,
            double restitutionCoefficient)
        {
            try
            {
                int isTargetStaticInt = isTargetStatic ? 1 : 0;
                return drone_service_rc_put_impulse_by_collision(
                    index,
                    isTargetStaticInt,
                    targetVelocity,
                    targetAngularVelocity,
                    target_euler,
                    selfContactVector,
                    targetContactVector,
                    targetInertia,
                    normal,
                    targetMass,
                    restitutionCoefficient);
            }
            catch (DllNotFoundException e)
            {
                Debug.LogError($"DllNotFoundException: {e.Message}");
                return -1;
            }
            catch (EntryPointNotFoundException e)
            {
                Debug.LogError($"EntryPointNotFoundException: {e.Message}");
                return -1;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Exception: {e.Message}");
                return -1;
            }
        }
        /*
         * Miscellaneous
         */
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong drone_service_rc_get_time_usec(int index);

        public static ulong GetTimeUsec(int index)
        {
            return drone_service_rc_get_time_usec(index);
        }
        /* ----------------------------------------------------------------
         *  Disturbance (Boundary)   ★ Added on 2025-07-12  (ROS ENU)
         * ----------------------------------------------------------------*/
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_put_disturbance_boundary(
            int index,
            double b_point_x, double b_point_y, double b_point_z,
            double b_normal_x, double b_normal_y, double b_normal_z);

        /// <summary>
        /// Defines a planar disturbance boundary in the ROS ENU frame (x-forward, y-left, z-up).
        /// </summary>
        /// <param name="index">Drone index</param>
        /// <param name="rosBoundaryPoint">Any point on the boundary plane (ROS ENU)</param>
        /// <param name="rosBoundaryNormal">Plane normal vector (ROS ENU)</param>
        /// <returns>0 on success, negative on error</returns>
        public static int PutDisturbanceBoundary(
            int index,
            Vector3 rosBoundaryPoint,
            Vector3 rosBoundaryNormal)
        {
            try
            {
                return drone_service_rc_put_disturbance_boundary(
                    index,
                    rosBoundaryPoint.x, rosBoundaryPoint.y, rosBoundaryPoint.z,
                    rosBoundaryNormal.x, rosBoundaryNormal.y, rosBoundaryNormal.z);
            }
            catch (DllNotFoundException e) { Debug.LogError($"DllNotFoundException: {e.Message}"); }
            catch (EntryPointNotFoundException e) { Debug.LogError($"EntryPointNotFoundException: {e.Message}"); }
            catch (Exception e) { Debug.LogError($"Exception: {e.Message}"); }
            return -1;
        }

        /* ----------------------------------------------------------------
         *  Propeller Wind            ★ Added on 2025-07-12  (ROS ENU)
         * ----------------------------------------------------------------*/
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int drone_service_rc_get_propeller_wind(
            int index,
            out double x, out double y, out double z);

        /// <summary>
        /// Retrieves the airflow velocity generated by the propellers, expressed in the ROS ENU frame (m/s).
        /// </summary>
        /// <param name="index">Drone index</param>
        /// <param name="rosWind">Output wind vector (ROS ENU)</param>
        /// <returns>0 on success, negative on error</returns>
        public static int GetPropellerWind(int index, out Vector3 rosWind)
        {
            rosWind = Vector3.zero;
            try
            {
                double x, y, z;
                int result = drone_service_rc_get_propeller_wind(index, out x, out y, out z);
                rosWind = new Vector3((float)x, (float)y, (float)z);
                return result;
            }
            catch (DllNotFoundException e) { Debug.LogError($"DllNotFoundException: {e.Message}"); }
            catch (EntryPointNotFoundException e) { Debug.LogError($"EntryPointNotFoundException: {e.Message}"); }
            catch (Exception e) { Debug.LogError($"Exception: {e.Message}"); }
            return -1;
        }


    }
}
