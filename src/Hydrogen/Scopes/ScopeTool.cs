//-----------------------------------------------------------------------
// <copyright file="ScopeTool.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools {
	public static class Scope {

        public static IDisposable ExecuteOnDispose(Action action) {
            return new ActionScope(action);
        }

        public static IAsyncDisposable ExecuteOnDisposeAsync(Func<Task> task) {
	        return new TaskScope(task);
        }

        public static IDisposable ExecuteOnDispose<T>(Action<T> action, T usingThesePrefetchedValues) {
            return new ActionScope(() => action(usingThesePrefetchedValues));
        }
		
        public static IAsyncDisposable ExecuteOnDispose<T>(Func<T, Task> task, T usingThesePrefetchedValues) {
	        return new TaskScope(() => task(usingThesePrefetchedValues));
        }

        public static IDisposable DeleteFileOnDispose(string filename)
	        => ExecuteOnDispose(() => File.Delete(filename));

        public static IAsyncDisposable DeleteFileOnDisposeAsync(string filename)
	        => ExecuteOnDisposeAsync(() =>  Tools.FileSystem.DeleteFileAsync(filename));

        public static IDisposable DeleteDirOnDispose(string directory)
	        => ExecuteOnDispose(() => Tools.FileSystem.DeleteDirectories(directory));
		
        public static IAsyncDisposable DeleteDirOnDisposeAsync(string directory)
	        => ExecuteOnDisposeAsync(() => Tools.FileSystem.DeleteDirectoriesAsync(directory));
	}

}
