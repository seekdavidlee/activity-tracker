# Introduction

Activity Tracker is used to help me track the time it takes for me to perform certain activities. It is designed to be hosted locally. It uses [File system API](https://github.com/seekdavidlee/filesystem-api) to store data.

The following is an example in `appsettings.sample.json`. Rename it to `appsettings.json` and modify the `StorageUri`.

```bash
{
	"StorageUri": "http://<filesystemapi endpoint>"
}
```

```bash
docker build -f ./src/Dockerfile -t activitytracker .
docker run -d -p 80:80 activitytracker
```