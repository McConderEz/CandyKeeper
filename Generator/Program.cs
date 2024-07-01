using CandyKeeper.DAL;
using CandyKeeper.DAL.Repositories;
using CandyKeeper.Domain.Models;
using Generator;
using Microsoft.EntityFrameworkCore;

var generator = new NotesGenerator();
generator.GenAll();