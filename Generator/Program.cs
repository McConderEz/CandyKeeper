using AutoMapper;
using CandyKeeper.DAL;
using CandyKeeper.DAL.Repositories;
using CandyKeeper.Domain.Models;
using Generator;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main(string[] args)
    {
        var generator = new NotesGenerator();
        generator.GenAll();
    }
}

