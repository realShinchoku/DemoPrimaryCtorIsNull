using AutoMapper;
using Contracts;
using StudentService.DTOs;
using StudentService.Models;

namespace StudentService.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Student, StudentDto>();

        CreateMap<StudentAuth, Student>();

        CreateMap<Student, StudentAuth>();

        CreateMap<StudentUpdateDto, Student>();

        CreateMap<Subject, SubjectDto>();
    }
}