﻿using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using CottonHarvesterHIDFileImportPlugin.PublisherDataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CottonHarvesterHIDFileImportPlugin.DataMappers
{
    public static class DataMapper
    {
        public static void MapData(Data myDataModel, ApplicationDataModel adm)
        {
            //Create an ADM Documents object
            Documents docs = new Documents();
            adm.Documents = docs;

            //Create a new observation object for our Documents object
            Observations observations = new Observations();
            var _observations = new List<Observations>();
            adm.Documents.Observations = _observations;

            //Create an ObsDataSet for our Documents object
            ObsDataset obsDataset = new ObsDataset();

            //Create an ADAPT container and load list object
            ContextItem parentContextItem = new ContextItem();
            var _loads = new List<AgGateway.ADAPT.ApplicationDataModel.LoggedData.Load>();
            var _obsCollections = new List<AgGateway.ADAPT.ApplicationDataModel.Documents.ObsCollection>();
            var _obs = new List<AgGateway.ADAPT.ApplicationDataModel.Documents.Obs>();

            //Map Client, Farm and Field to ADM
            MapGrowerData(myDataModel, adm.Catalog);

            //Import the Loads and Context Items
            foreach (HIDRecord ndb in myDataModel.HIDData.HIDRecords)
            {
                //IMPORTANT! Each one of these obsCollection iterations are for a unique load
                AgGateway.ADAPT.ApplicationDataModel.Documents.ObsCollection obsCollection = new AgGateway.ADAPT.ApplicationDataModel.Documents.ObsCollection();
                AgGateway.ADAPT.ApplicationDataModel.LoggedData.Load load = new AgGateway.ADAPT.ApplicationDataModel.LoggedData.Load();

                //We create a new load for each unique record
                load = HIDRecordMapper.MapHIDRecord(ndb, load);
                
                //We create a new Observation Collection for each unique load
                obsCollection = HIDRecordMapper.MapHIDRecordObsCollection(ndb, _obs, load.Id.ReferenceId, obsCollection, obsDataset, adm);
                _obsCollections.Add(obsCollection);
                obsDataset.ObsCollectionIds.Add(obsCollection.Id.ReferenceId);
                load.ObsCollectionId = obsCollection.Id.ReferenceId;

                _loads.Add(load);
            }

            var _obsDataSets = new List<ObsDataset>();
            _obsDataSets.Add(obsDataset);
            adm.Documents.ObsDatasets = _obsDataSets;
            adm.Documents.ObsCollections = _obsCollections;
            adm.Documents.Obs = _obs;

            adm.Documents.Loads = _loads;
        }

        public static void MapGrowerData(Data myDataModel, Catalog catalog)
        {
            //Import the clients, farms and fields
            foreach (Client myClient in myDataModel.Clients)
            {
                ClientFarmFieldMapper.MapGrower(myClient, catalog);
            }
        }

        public static UniqueId GetNativeID(BaseObject obj)
        {
            UniqueId id = new UniqueId();
            id.Id = obj.ID.ToString();
            id.IdType = IdTypeEnum.UUID;
            id.Source = "CottonInc.JohnDeereHIDPlugin";
            id.SourceType = IdSourceTypeEnum.URI;
            return id;
        }
    }
}
