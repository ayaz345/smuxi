/*
 * Smuxi - Smart MUltipleXed Irc
 *
 * Copyright (c) 2023 Mirco Bauer <meebey@meebey.net>
 *
 * Full GPL License: <http://www.gnu.org/licenses/gpl.txt>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307 USA
 */

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

// The TargetFramework attribute is used and needed by the Microsoft .NET runtime on Windows.
// The value determines which framework version will be used at runtime.
// On the Mono runtime this attribute has no effect.
//
// From https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-target-framework-and-target-platform?source=recommendations&view=vs-2022
// The available values for TargetFrameworkVersion are:
// v2.0, v3.0, v3.5, v4.5.2, v4.6, v4.6.1, v4.6.2, v4.7, v4.7.1, v4.7.2, and v4.8.
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework, Version 4.7.2")]
