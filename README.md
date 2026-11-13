# WBS Coding School

Course exercises and module projects from WBS Coding School.

## Module project

**[ChallengeTracker](ChallengeTracker/)** — a fullstack challenge tracker built with ASP.NET Core 10, EF Core, SQLite, React 19 and TypeScript. Includes accounts, public/private challenges, memberships, daily progress and a leaderboard.

- [Setup and project overview](ChallengeTracker/README.md)
- [Requirements checklist](ChallengeTracker/docs/REQUIREMENTS.md)
- [Test report](ChallengeTracker/docs/TEST_REPORT.md)
- [API endpoints](ChallengeTracker/docs/ENDPOINTS.md)

```bash
git clone https://github.com/pavlis29/wbs-coding-school.git
cd wbs-coding-school/ChallengeTracker
./start.sh
```

Requires .NET 10 SDK and Node.js 22+. Local data and credentials are generated on first start and are not stored in this repository.

## Course chapters

| Folder | Contents |
| --- | --- |
| [chapter_2](chapter_2/) | Python fundamentals, data structures and algorithms |
| [chapter_7](chapter_7/) | HTML and CSS exercises |
| [chapter_19](chapter_19/) | C# fundamentals and UML |

Each project keeps its own source files and documentation. ChallengeTracker's Docker/SQL Server setup is included; its runtime verification is recorded separately in the test report.
