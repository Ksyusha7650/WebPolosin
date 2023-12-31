﻿using Database.Models;
using MySql.Data.MySqlClient;

namespace Database;

public class DataChannel
{
    private readonly BaseRepository _baseRepository;

    public DataChannel(BaseRepository baseRepository)
    {
        _baseRepository = baseRepository;
    }

    public string[] GetMarks()
    {
        const string sqlQuery = @"
select Mark
from channel
";
        using var connection = _baseRepository.GetAndOpenConnection().Result;
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        List<string> marks = new();
        while (reader.Read()) marks.Add(reader.GetString(0));
        return marks.ToArray();
    }

    public async Task<GeometricParametersModel> GetGeometricParameters(string mark)
    {
        const string sqlQuery = @"
select ID_ParameterSet
from channel
where Mark = @Mark
";
        await using var connection = await _baseRepository.GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@Mark", mark);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        var idProSet = 0;
        while (reader.Read())
            idProSet = reader.GetInt32(0);
        var values = await _baseRepository.GetDataByPropertySet(idProSet);
        return new GeometricParametersModel(values[0], values[1], values[2]);
    }

    public async void SetGeometricParameters(
        string mark,
        GeometricParametersModel geometricParametersModel)
    {
        var idParameterSet = await _baseRepository.AddNewParameterSet();
        var idUnit = await _baseRepository.GetIdUnit("m");
        _baseRepository.AddParameterInParameterSet(
            idParameterSet,
            await _baseRepository.GetIdParameter("Height"),
            idUnit,
            geometricParametersModel.Height);
        _baseRepository.AddParameterInParameterSet(
            idParameterSet,
            await _baseRepository.GetIdParameter("Width"),
            idUnit,
            geometricParametersModel.Width);
        _baseRepository.AddParameterInParameterSet(
            idParameterSet,
            await _baseRepository.GetIdParameter("Length"),
            idUnit,
            geometricParametersModel.Length);

        const string sqlQuery = @"
insert into channel (ID_ParameterSet, Mark)
VALUES (@IdParameterSet, @Mark);
";
        await using var connection = await _baseRepository.GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@IdParameterSet", idParameterSet);
        cmd.Parameters.AddWithValue("@Mark", mark);
        cmd.CommandText = sqlQuery;
        cmd.ExecuteNonQuery();
    }

    public int GetIdParameterSet(string name)
    {
        const string sqlQuery = @"
select ID_ParameterSet
from channel
where Mark = @MarkName;
";
        using var connection = _baseRepository.GetAndOpenConnection().Result;
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@MarkName", name);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        var idMaterial = 0;
        while (reader.Read())
            idMaterial = reader.GetInt32(0);
        return idMaterial;
    }

    public async void EditChannel(GeometricParametersModel channel, string mark)
    {
        var idParameterSet = GetIdParameterSet(mark);
        _baseRepository.UpdateParameterInParameterSet(
            idParameterSet,
            await _baseRepository.GetIdParameter("Height"),
            channel.Height);
        _baseRepository.UpdateParameterInParameterSet(
            idParameterSet,
            await _baseRepository.GetIdParameter("Length"),
            channel.Length);
        _baseRepository.UpdateParameterInParameterSet(
            idParameterSet,
            await _baseRepository.GetIdParameter("Width"),
            channel.Width);
    }

    public async void DeleteChannel(string mark)
    {
        var idParameterSet = GetIdParameterSet(mark);
        _baseRepository.DeleteParameterSet(idParameterSet);
        const string sqlQuery = @"
delete from channel WHERE (`ID_ParameterSet` = @IdParameterSet);
";
        await using var connection = await _baseRepository.GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@IdParameterSet", idParameterSet);
        cmd.CommandText = sqlQuery;
        cmd.ExecuteNonQuery();
    }
}