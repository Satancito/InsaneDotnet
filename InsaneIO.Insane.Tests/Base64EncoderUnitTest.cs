using InsaneIO.Insane.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using InsaneIO.Insane.Extensions;
using System.Text.Json.Nodes;
using System.Security.Cryptography;
using System.Runtime.Versioning;
using FluentAssertions;
using FluentAssertions.Equivalency;
using System.Text.RegularExpressions;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class Base64EncoderUnitTest
    {
        public readonly static string resultWith2Pad = "QQ==";
        public readonly static string resultWith1Pad = "QUE=";
        public readonly static string resultWith0Pad = "QUFB";

        public readonly static string resultWith2PadRemoved = "QQ";
        public readonly static string resultWith1PadRemoved = "QUE";
        public readonly static string resultWith0PadRemoved = "QUFB";

        public readonly static string inputFor2Pad = "A";
        public readonly static string inputFor1Pad = "AA";
        public readonly static string inputFor0Pad = "AAA";
        
        public readonly static byte[] bytesFor2Pad = { 65 };
        public readonly static byte[] bytesFor1Pad = { 65, 65 };
        public readonly static byte[] bytesFor0Pad = { 65, 65, 65 };

        byte[] TestBytes = { 0x30, 0x82, 0x02, 0x22, 0x30, 0x0d, 0x06, 0x09, 0x2a, 0x86, 0x48
        , 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01, 0x05, 0x00, 0x03, 0x82
        , 0x02, 0x0f, 0x00, 0x30, 0x82, 0x02, 0x0a, 0x02, 0x82, 0x02
        , 0x01, 0x00, 0xf2, 0xe8, 0xe5, 0x81, 0x32, 0x36, 0xb8, 0xb6
        , 0x3f, 0xb5, 0xbe, 0x76, 0x65, 0x65, 0xd1, 0x8f, 0x2d, 0xc4
        , 0xc5, 0xa1, 0x91, 0x3b, 0x8b, 0xdc, 0x8b, 0xf6, 0x4f, 0x42
        , 0x64, 0xd1, 0xea, 0xdc, 0x75, 0x6c, 0x83, 0x0b, 0x81, 0x1f
        , 0x57, 0xeb, 0xac, 0xe5, 0xd0, 0x5c, 0x6b, 0x5f, 0x37, 0xa8
        , 0x53, 0x1c, 0x65, 0x6b, 0x75, 0x5e, 0xbc, 0xd3, 0x59, 0xd2
        , 0x54, 0x17, 0xf7, 0x69, 0x4d, 0x23, 0x92, 0x7e, 0x78, 0x47
        , 0xf1, 0x06, 0x04, 0x5b, 0x55, 0x00, 0xb1, 0xaa, 0x82, 0x70
        , 0x70, 0xc0, 0xff, 0x3c, 0x29, 0x4a, 0x2f, 0xc3, 0xff, 0x56
        , 0x60, 0x4a, 0x22, 0x12, 0xfe, 0x10, 0xa4, 0xe1, 0xeb, 0x9d
        , 0x82, 0xb3, 0x76, 0x1c, 0xa0, 0x18, 0x4c, 0xca, 0xcd, 0x68
        , 0x40, 0x2e, 0x6a, 0x21, 0x2a, 0x7b, 0x7b, 0xc6, 0x0b, 0x85
        , 0x14, 0x19, 0x03, 0x40, 0xe9, 0x78, 0x54, 0xfe, 0x97, 0xf4
        , 0xe8, 0x39, 0x45, 0x06, 0x76, 0x8e, 0x5e, 0x0e, 0xdb, 0x62
        , 0x41, 0x60, 0x2b, 0xfb, 0x1e, 0x1a, 0x65, 0x3a, 0x25, 0x48
        , 0xba, 0xe6, 0x73, 0x8f, 0x35, 0xf0, 0xfd, 0x99, 0xe4, 0x1d
        , 0xe9, 0xbf, 0x67, 0x8b, 0xf4, 0x1d, 0xfa, 0xfa, 0x58, 0x8e
        , 0xe7, 0x1b, 0x7b, 0xb5, 0x7d, 0x74, 0x90, 0x26, 0x41, 0x88
        , 0xbd, 0x4d, 0x20, 0x69, 0x4b, 0x4c, 0x8a, 0xef, 0x47, 0x87
        , 0xc1, 0xf3, 0x5b, 0x42, 0x79, 0x04, 0xd7, 0x9d, 0x42, 0xa7
        , 0xdf, 0xca, 0x0d, 0xf4, 0x19, 0x4a, 0x8d, 0x7c, 0x93, 0x3f
        , 0x1a, 0xa5, 0x39, 0xef, 0xcd, 0x6d, 0xe5, 0x0a, 0xe5, 0xf0
        , 0x41, 0x16, 0x96, 0x58, 0x14, 0x99, 0x77, 0xdc, 0x69, 0x27
        , 0xc7, 0xa6, 0x11, 0xb4, 0xd3, 0xa2, 0x17, 0x23, 0x50, 0xa0
        , 0xbd, 0x06, 0x7d, 0x5a, 0x72, 0xa0, 0xb1, 0xed, 0x48, 0xd1
        , 0x42, 0xfc, 0x66, 0x3e, 0x4a, 0x22, 0x69, 0xac, 0xe4, 0xee
        , 0x82, 0xbc, 0x48, 0x83, 0x81, 0x34, 0x6e, 0x29, 0x4b, 0x64
        , 0x71, 0x37, 0x25, 0x13, 0x28, 0x52, 0x71, 0x5b, 0xd5, 0x95
        , 0x20, 0xa5, 0xb4, 0x66, 0xa7, 0x9e, 0x06, 0x5f, 0x2d, 0x8e
        , 0x78, 0xf5, 0x37, 0xcf, 0xed, 0x65, 0x84, 0xdf, 0xda, 0x78
        , 0x27, 0xa9, 0x09, 0xaa, 0x70, 0x73, 0x5a, 0xc6, 0xa9, 0xba
        , 0xb7, 0xce, 0x38, 0x2c, 0x28, 0x4b, 0x3e, 0xae, 0x11, 0x3c
        , 0xed, 0x94, 0xd9, 0x2a, 0x26, 0xd2, 0xbc, 0xa5, 0x19, 0x7c
        , 0x3a, 0x98, 0x0a, 0x51, 0xdb, 0x14, 0x99, 0xd8, 0x4e, 0xc3
        , 0x5d, 0x0a, 0xc9, 0x93, 0xa9, 0xce, 0xb0, 0x12, 0x62, 0x6b
        , 0x6b, 0x48, 0x42, 0x42, 0x04, 0x95, 0x29, 0x77, 0x49, 0xaa
        , 0x88, 0x2a, 0x94, 0xcd, 0x55, 0x7d, 0xb6, 0xcb, 0xb1, 0x1e
        , 0x93, 0xa9, 0xa2, 0xba, 0x73, 0xff, 0x2e, 0xa6, 0xff, 0xd6
        , 0x14, 0x65, 0x3b, 0x8c, 0x7d, 0x0b, 0xa7, 0xda, 0xbd, 0x50
        , 0x2c, 0x1d, 0x2e, 0xf1, 0xd9, 0xf5, 0x8a, 0x08, 0xe9, 0x54
        , 0x7d, 0x4a, 0x25, 0xf5, 0xb7, 0x53, 0xd8, 0x3f, 0xad, 0x98
        , 0x5f, 0xea, 0xa9, 0xd5, 0x3d, 0x13, 0x7d, 0x26, 0x5d, 0xab
        , 0x0e, 0xa6, 0xcd, 0xe7, 0xc1, 0x81, 0x0f, 0x12, 0x8c, 0x59
        , 0x77, 0xa9, 0x67, 0xa4, 0x37, 0xf3, 0x8e, 0xdf, 0xe5, 0x5c
        , 0x0c, 0x65, 0x07, 0x93, 0xcd, 0xb0, 0xeb, 0x19, 0x89, 0x6f
        , 0x81, 0x90, 0x9a, 0xf4, 0x99, 0xb8, 0x33, 0x35, 0xdb, 0x40
        , 0x8e, 0x85, 0x53, 0x26, 0x4a, 0xe9, 0x8c, 0x5a, 0x5d, 0x68
        , 0xd5, 0x4e, 0xff, 0x21, 0x77, 0xb9, 0xcb, 0xc1, 0xaf, 0x69
        , 0x69, 0x10, 0x56, 0x6d, 0x9e, 0xbd, 0xe4, 0xa4, 0x2b, 0xd9
        , 0xf9, 0x65, 0x63, 0xb5, 0x00, 0x48, 0xb0, 0x04, 0xca, 0x98
        , 0x10, 0x8e, 0x2a, 0x4f, 0x18, 0x47, 0xef, 0x5e, 0x26, 0x07
        , 0x72, 0xf9, 0xbe, 0x25, 0x02, 0x03, 0x01, 0x00, 0x01 };

        public readonly static string TestMimeBase64String = @"MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA8ujlgTI2uLY/tb52ZWXRjy3ExaGRO4vc
i/ZPQmTR6tx1bIMLgR9X66zl0FxrXzeoUxxla3VevNNZ0lQX92lNI5J+eEfxBgRbVQCxqoJwcMD/
PClKL8P/VmBKIhL+EKTh652Cs3YcoBhMys1oQC5qISp7e8YLhRQZA0DpeFT+l/ToOUUGdo5eDtti
QWAr+x4aZTolSLrmc4818P2Z5B3pv2eL9B36+liO5xt7tX10kCZBiL1NIGlLTIrvR4fB81tCeQTX
nUKn38oN9BlKjXyTPxqlOe/NbeUK5fBBFpZYFJl33Gknx6YRtNOiFyNQoL0GfVpyoLHtSNFC/GY+
SiJprOTugrxIg4E0bilLZHE3JRMoUnFb1ZUgpbRmp54GXy2OePU3z+1lhN/aeCepCapwc1rGqbq3
zjgsKEs+rhE87ZTZKibSvKUZfDqYClHbFJnYTsNdCsmTqc6wEmJra0hCQgSVKXdJqogqlM1VfbbL
sR6TqaK6c/8upv/WFGU7jH0Lp9q9UCwdLvHZ9YoI6VR9SiX1t1PYP62YX+qp1T0TfSZdqw6mzefB
gQ8SjFl3qWekN/OO3+VcDGUHk82w6xmJb4GQmvSZuDM120COhVMmSumMWl1o1U7/IXe5y8GvaWkQ
Vm2eveSkK9n5ZWO1AEiwBMqYEI4qTxhH714mB3L5viUCAwEAAQ==";

        public readonly static string TestPemBase64String = @"MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA8ujlgTI2uLY/tb52ZWXR
jy3ExaGRO4vci/ZPQmTR6tx1bIMLgR9X66zl0FxrXzeoUxxla3VevNNZ0lQX92lN
I5J+eEfxBgRbVQCxqoJwcMD/PClKL8P/VmBKIhL+EKTh652Cs3YcoBhMys1oQC5q
ISp7e8YLhRQZA0DpeFT+l/ToOUUGdo5eDttiQWAr+x4aZTolSLrmc4818P2Z5B3p
v2eL9B36+liO5xt7tX10kCZBiL1NIGlLTIrvR4fB81tCeQTXnUKn38oN9BlKjXyT
PxqlOe/NbeUK5fBBFpZYFJl33Gknx6YRtNOiFyNQoL0GfVpyoLHtSNFC/GY+SiJp
rOTugrxIg4E0bilLZHE3JRMoUnFb1ZUgpbRmp54GXy2OePU3z+1lhN/aeCepCapw
c1rGqbq3zjgsKEs+rhE87ZTZKibSvKUZfDqYClHbFJnYTsNdCsmTqc6wEmJra0hC
QgSVKXdJqogqlM1VfbbLsR6TqaK6c/8upv/WFGU7jH0Lp9q9UCwdLvHZ9YoI6VR9
SiX1t1PYP62YX+qp1T0TfSZdqw6mzefBgQ8SjFl3qWekN/OO3+VcDGUHk82w6xmJ
b4GQmvSZuDM120COhVMmSumMWl1o1U7/IXe5y8GvaWkQVm2eveSkK9n5ZWO1AEiw
BMqYEI4qTxhH714mB3L5viUCAwEAAQ==";
        public readonly static string TestBase64String = "MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA8ujlgTI2uLY/tb52ZWXRjy3ExaGRO4vci/ZPQmTR6tx1bIMLgR9X66zl0FxrXzeoUxxla3VevNNZ0lQX92lNI5J+eEfxBgRbVQCxqoJwcMD/PClKL8P/VmBKIhL+EKTh652Cs3YcoBhMys1oQC5qISp7e8YLhRQZA0DpeFT+l/ToOUUGdo5eDttiQWAr+x4aZTolSLrmc4818P2Z5B3pv2eL9B36+liO5xt7tX10kCZBiL1NIGlLTIrvR4fB81tCeQTXnUKn38oN9BlKjXyTPxqlOe/NbeUK5fBBFpZYFJl33Gknx6YRtNOiFyNQoL0GfVpyoLHtSNFC/GY+SiJprOTugrxIg4E0bilLZHE3JRMoUnFb1ZUgpbRmp54GXy2OePU3z+1lhN/aeCepCapwc1rGqbq3zjgsKEs+rhE87ZTZKibSvKUZfDqYClHbFJnYTsNdCsmTqc6wEmJra0hCQgSVKXdJqogqlM1VfbbLsR6TqaK6c/8upv/WFGU7jH0Lp9q9UCwdLvHZ9YoI6VR9SiX1t1PYP62YX+qp1T0TfSZdqw6mzefBgQ8SjFl3qWekN/OO3+VcDGUHk82w6xmJb4GQmvSZuDM120COhVMmSumMWl1o1U7/IXe5y8GvaWkQVm2eveSkK9n5ZWO1AEiwBMqYEI4qTxhH714mB3L5viUCAwEAAQ==";
        public readonly static string TestBase64StringNoPadding = "MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA8ujlgTI2uLY/tb52ZWXRjy3ExaGRO4vci/ZPQmTR6tx1bIMLgR9X66zl0FxrXzeoUxxla3VevNNZ0lQX92lNI5J+eEfxBgRbVQCxqoJwcMD/PClKL8P/VmBKIhL+EKTh652Cs3YcoBhMys1oQC5qISp7e8YLhRQZA0DpeFT+l/ToOUUGdo5eDttiQWAr+x4aZTolSLrmc4818P2Z5B3pv2eL9B36+liO5xt7tX10kCZBiL1NIGlLTIrvR4fB81tCeQTXnUKn38oN9BlKjXyTPxqlOe/NbeUK5fBBFpZYFJl33Gknx6YRtNOiFyNQoL0GfVpyoLHtSNFC/GY+SiJprOTugrxIg4E0bilLZHE3JRMoUnFb1ZUgpbRmp54GXy2OePU3z+1lhN/aeCepCapwc1rGqbq3zjgsKEs+rhE87ZTZKibSvKUZfDqYClHbFJnYTsNdCsmTqc6wEmJra0hCQgSVKXdJqogqlM1VfbbLsR6TqaK6c/8upv/WFGU7jH0Lp9q9UCwdLvHZ9YoI6VR9SiX1t1PYP62YX+qp1T0TfSZdqw6mzefBgQ8SjFl3qWekN/OO3+VcDGUHk82w6xmJb4GQmvSZuDM120COhVMmSumMWl1o1U7/IXe5y8GvaWkQVm2eveSkK9n5ZWO1AEiwBMqYEI4qTxhH714mB3L5viUCAwEAAQ";
        public readonly static string TestUrlSafeBase64String = "MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA8ujlgTI2uLY_tb52ZWXRjy3ExaGRO4vci_ZPQmTR6tx1bIMLgR9X66zl0FxrXzeoUxxla3VevNNZ0lQX92lNI5J-eEfxBgRbVQCxqoJwcMD_PClKL8P_VmBKIhL-EKTh652Cs3YcoBhMys1oQC5qISp7e8YLhRQZA0DpeFT-l_ToOUUGdo5eDttiQWAr-x4aZTolSLrmc4818P2Z5B3pv2eL9B36-liO5xt7tX10kCZBiL1NIGlLTIrvR4fB81tCeQTXnUKn38oN9BlKjXyTPxqlOe_NbeUK5fBBFpZYFJl33Gknx6YRtNOiFyNQoL0GfVpyoLHtSNFC_GY-SiJprOTugrxIg4E0bilLZHE3JRMoUnFb1ZUgpbRmp54GXy2OePU3z-1lhN_aeCepCapwc1rGqbq3zjgsKEs-rhE87ZTZKibSvKUZfDqYClHbFJnYTsNdCsmTqc6wEmJra0hCQgSVKXdJqogqlM1VfbbLsR6TqaK6c_8upv_WFGU7jH0Lp9q9UCwdLvHZ9YoI6VR9SiX1t1PYP62YX-qp1T0TfSZdqw6mzefBgQ8SjFl3qWekN_OO3-VcDGUHk82w6xmJb4GQmvSZuDM120COhVMmSumMWl1o1U7_IXe5y8GvaWkQVm2eveSkK9n5ZWO1AEiwBMqYEI4qTxhH714mB3L5viUCAwEAAQ";
        public readonly static string TestFileNameSafeBase64String = "MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA8ujlgTI2uLY_tb52ZWXRjy3ExaGRO4vci_ZPQmTR6tx1bIMLgR9X66zl0FxrXzeoUxxla3VevNNZ0lQX92lNI5J-eEfxBgRbVQCxqoJwcMD_PClKL8P_VmBKIhL-EKTh652Cs3YcoBhMys1oQC5qISp7e8YLhRQZA0DpeFT-l_ToOUUGdo5eDttiQWAr-x4aZTolSLrmc4818P2Z5B3pv2eL9B36-liO5xt7tX10kCZBiL1NIGlLTIrvR4fB81tCeQTXnUKn38oN9BlKjXyTPxqlOe_NbeUK5fBBFpZYFJl33Gknx6YRtNOiFyNQoL0GfVpyoLHtSNFC_GY-SiJprOTugrxIg4E0bilLZHE3JRMoUnFb1ZUgpbRmp54GXy2OePU3z-1lhN_aeCepCapwc1rGqbq3zjgsKEs-rhE87ZTZKibSvKUZfDqYClHbFJnYTsNdCsmTqc6wEmJra0hCQgSVKXdJqogqlM1VfbbLsR6TqaK6c_8upv_WFGU7jH0Lp9q9UCwdLvHZ9YoI6VR9SiX1t1PYP62YX-qp1T0TfSZdqw6mzefBgQ8SjFl3qWekN_OO3-VcDGUHk82w6xmJb4GQmvSZuDM120COhVMmSumMWl1o1U7_IXe5y8GvaWkQVm2eveSkK9n5ZWO1AEiwBMqYEI4qTxhH714mB3L5viUCAwEAAQ";
        public readonly static string TestUrlEncodedBase64String = "MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA8ujlgTI2uLY%2Ftb52ZWXRjy3ExaGRO4vci%2FZPQmTR6tx1bIMLgR9X66zl0FxrXzeoUxxla3VevNNZ0lQX92lNI5J%2BeEfxBgRbVQCxqoJwcMD%2FPClKL8P%2FVmBKIhL%2BEKTh652Cs3YcoBhMys1oQC5qISp7e8YLhRQZA0DpeFT%2Bl%2FToOUUGdo5eDttiQWAr%2Bx4aZTolSLrmc4818P2Z5B3pv2eL9B36%2BliO5xt7tX10kCZBiL1NIGlLTIrvR4fB81tCeQTXnUKn38oN9BlKjXyTPxqlOe%2FNbeUK5fBBFpZYFJl33Gknx6YRtNOiFyNQoL0GfVpyoLHtSNFC%2FGY%2BSiJprOTugrxIg4E0bilLZHE3JRMoUnFb1ZUgpbRmp54GXy2OePU3z%2B1lhN%2FaeCepCapwc1rGqbq3zjgsKEs%2BrhE87ZTZKibSvKUZfDqYClHbFJnYTsNdCsmTqc6wEmJra0hCQgSVKXdJqogqlM1VfbbLsR6TqaK6c%2F8upv%2FWFGU7jH0Lp9q9UCwdLvHZ9YoI6VR9SiX1t1PYP62YX%2Bqp1T0TfSZdqw6mzefBgQ8SjFl3qWekN%2FOO3%2BVcDGUHk82w6xmJb4GQmvSZuDM120COhVMmSumMWl1o1U7%2FIXe5y8GvaWkQVm2eveSkK9n5ZWO1AEiwBMqYEI4qTxhH714mB3L5viUCAwEAAQ%3D%3D";
        public Base64Encoder encoder = Base64Encoder.DefaultInstance;
        public Base64Encoder encoderNoPadding = new() { RemovePadding = true };
        public Base64Encoder encoderWithPadding = new() { RemovePadding = false };
        public Base64Encoder encoderWithPaddingNormalBase64 = new() { EncodingType= Base64Encoding.Base64 };
        public Base64Encoder encoderWithPaddingUrlSafeBase64 = new() { EncodingType = Base64Encoding.UrlSafeBase64 };
        public Base64Encoder encoderWithPaddingFileNameSafeBase64 = new() { EncodingType = Base64Encoding.FileNameSafeBase64 };
        public Base64Encoder encoderWithPaddingUrlEncodedBase64 = new() { EncodingType = Base64Encoding.UrlEncodedBase64 };
        public Base64Encoder encoderNoPaddingNormalBase64 = new() { EncodingType = Base64Encoding.Base64, RemovePadding = true };
        public Base64Encoder encoderMimeLineBreaksNormalBase64 = new() { EncodingType = Base64Encoding.Base64, LineBreaksLength = Base64Encoder.MimeLineBreaksLength };
        public Base64Encoder encoderPemLineBreaksNormalBase64 = new() { EncodingType = Base64Encoding.Base64, LineBreaksLength = Base64Encoder.PemLineBreaksLength };

        [TestMethod]
        public void TestEncodeFor2Pad()
        {
            Assert.AreEqual(resultWith2Pad, encoder.Encode(inputFor2Pad.ToByteArrayUtf8()));
            Assert.AreEqual(resultWith2Pad, encoder.Encode(bytesFor2Pad));
        }

        [TestMethod]
        public void TestEncodeFor1Pad()
        {
            Assert.AreEqual(resultWith1Pad, encoder.Encode(inputFor1Pad.ToByteArrayUtf8()));
            Assert.AreEqual(resultWith1Pad, encoder.Encode(bytesFor1Pad));
        }

        [TestMethod]
        public void TestEncodeFor0Pad()
        {
            Assert.AreEqual(resultWith0Pad, encoder.Encode(inputFor0Pad.ToByteArrayUtf8()));
            Assert.AreEqual(resultWith0Pad, encoder.Encode(bytesFor0Pad));
        }

        [TestMethod]
        public void TestEncodeFor2PadRemoved()
        {
            Assert.AreEqual(resultWith2PadRemoved, encoderNoPadding.Encode(inputFor2Pad.ToByteArrayUtf8()));
            Assert.AreEqual(resultWith2PadRemoved, encoderNoPadding.Encode(bytesFor2Pad));
        }

        [TestMethod]
        public void TestEncodeFor1PadRemoved()
        {
            Assert.AreEqual(resultWith1PadRemoved, encoderNoPadding.Encode(inputFor1Pad.ToByteArrayUtf8()));
            Assert.AreEqual(resultWith1PadRemoved, encoderNoPadding.Encode(bytesFor1Pad));
        }

        [TestMethod]
        public void TestEncodeFor0PadRemoved()
        {
            Assert.AreEqual(resultWith0PadRemoved, encoderNoPadding.Encode(inputFor0Pad.ToByteArrayUtf8()));
            Assert.AreEqual(resultWith0PadRemoved, encoderNoPadding.Encode(bytesFor0Pad));
        }


        [TestMethod]
        public void TestEncode()
        {
            Assert.AreEqual(TestBase64String, encoder.Encode(TestBytes));
        }

        [TestMethod]
        public void TestEncodeBytesToFilenameSafe()
        {
            Assert.AreEqual(TestFileNameSafeBase64String, encoderWithPaddingFileNameSafeBase64.Encode(TestBytes));
        }

        [TestMethod]
        public void TestEncodeBytesToUrlSafe()
        {
            Assert.AreEqual(TestUrlSafeBase64String, encoderWithPaddingUrlSafeBase64.Encode(TestBytes));
        }

        [TestMethod]
        public void TestEncodeBytesToUrlEncoded()
        {
            Assert.AreEqual(TestUrlEncodedBase64String, encoderWithPaddingUrlEncodedBase64.Encode(TestBytes));
        }

        [TestMethod]
        public void TestEncodeWithNoPadding()
        {
            Assert.AreEqual(TestBase64StringNoPadding, encoderNoPaddingNormalBase64.Encode(TestBytes));
        }

        [TestMethod]
        public void TestDecodeBase64()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(TestBytes, encoder.Decode(TestBase64String)));
        }

        [TestMethod]
        public void TestDecodeBase64NoPadding()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(TestBytes, encoder.Decode(TestBase64StringNoPadding)));
        }

        [TestMethod]
        public void TestDecodeUrlSafeBase64()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(TestBytes, encoder.Decode(TestUrlSafeBase64String)));
        }

        [TestMethod]
        public void TestDecodeFileNameSafeBase64()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(TestBytes, encoder.Decode(TestFileNameSafeBase64String)));
        }

        [TestMethod]
        public void TestDecodeUrlEncodedBase64()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(TestBytes, encoder.Decode(TestUrlEncodedBase64String)));
        }

        public void TestEncodePemBase64()
        {
            Assert.AreEqual(TestPemBase64String, encoderPemLineBreaksNormalBase64.Encode(TestBytes));
        }

        [TestMethod]
        public void TestEncodeMimeBase64()
        {
            Assert.AreEqual(NormalizeNewLines(TestMimeBase64String), encoderMimeLineBreaksNormalBase64.Encode(TestBytes));
        }

        public void TestDecodePemBase64()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(TestBytes, encoderPemLineBreaksNormalBase64.Decode(TestPemBase64String)));
        }

        [TestMethod]
        public void TestDecodeMimeBase64()
        {
            TestBytes.Should().BeEquivalentTo(encoderMimeLineBreaksNormalBase64.Decode(TestMimeBase64String));
        }

        [TestMethod]
        public void TestSerializeDeserialize()
        {
            new Base64Encoder().Serialize().Should().NotBeNullOrWhiteSpace();

            Base64Encoder[] encoders =
            [
                Base64Encoder.DefaultInstance,
                encoderNoPaddingNormalBase64,
                encoderMimeLineBreaksNormalBase64,
                encoderPemLineBreaksNormalBase64,
                encoderWithPaddingUrlSafeBase64,
                encoderWithPaddingFileNameSafeBase64,
                encoderWithPaddingUrlEncodedBase64
            ];

            foreach (Base64Encoder encoder in encoders)
            {
                string json = encoder.Serialize(false);
                JsonObject jsonObject = encoder.ToJsonObject();
                IEncoder deserialized = Base64Encoder.Deserialize(json);
                Assert.AreEqual(encoder.GetType().FullName, deserialized.GetType().FullName);
                Assert.IsInstanceOfType(deserialized, typeof(Base64Encoder));
                TestSerializationAssertions.AssertJsonEquals(jsonObject, deserialized.ToJsonObject());
                deserialized.Encode(TestBytes).Should().Be(encoder.Encode(TestBytes));
            }
        }

        private static string NormalizeNewLines(string value)
        {
            return Regex.Replace(value, "\r?\n", Environment.NewLine);
        }
    }
}
